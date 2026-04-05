using IPL.Gaming.Attributes;
using IPL.Gaming.Common.Enums;
using IPL.Gaming.Common.Models.CosmosDB;
using IPL.Gaming.Services;
using IPL.Gaming.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace IPL.Gaming.Controllers
{
    [ApiKey]
    [Route("api/[controller]")]
    [ApiController]
    public class BetSettlementController : BaseController
    {
        private readonly IBetSettlementService _betSettlementService;
        private readonly ITransactionService _transactionService;
        private readonly IUserService _userService;
        private readonly UserCacheService _userCacheService;
        private readonly IQuestionService _questionService;
        private readonly IMatchStatusService _matchStatusService;

        public BetSettlementController(
            IBetSettlementService betSettlementService,
            ITransactionService transactionService,
            IUserService userService,
            UserCacheService userCacheService,
            IQuestionService questionService,
            IMatchStatusService matchStatusService)
        {
            _betSettlementService = betSettlementService;
            _transactionService   = transactionService;
            _userService          = userService;
            _userCacheService     = userCacheService;
            _questionService      = questionService;
            _matchStatusService   = matchStatusService;
        }

        // ── Inner DTO ─────────────────────────────────────────────────────────
        private record TransactionSummaryDto(
            Guid Id,
            Guid UserId,
            string UserName,
            Guid? MatchId,
            double OverallCreditChange,
            List<Change>? Changes,
            TransactionStatus Status,
            TransactionType Type,
            DateTime CreatedAt);

        /// <summary>
        /// Settles all bets for a match. Requires status = MatchCompleted and all
        /// questions to have correctOptionId set. Admin+.
        /// </summary>
        [HttpPost]
        [Route("SettleBets/{matchId}")]
        [RequireRole(UserRole.Admin, UserRole.SuperAdmin)]
        public async Task<IActionResult> SettleBets(Guid matchId)
        {
            try
            {
                await _betSettlementService.SettleBets(matchId);
                return Ok(new { message = $"Bets settled successfully for match {matchId}." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Returns all transactions for a match, enriched with user names. Admin+.
        /// </summary>
        [HttpGet]
        [Route("GetTransactionsByMatch/{matchId}")]
        [RequireRole(UserRole.Admin, UserRole.SuperAdmin)]
        public async Task<IActionResult> GetTransactionsByMatch(Guid matchId)
        {
            try
            {
                var transactions = await _transactionService.GetTransactionsByMatchId(matchId);
                var users        = await _userService.GetAllUsers();
                var userDict     = users.ToDictionary(u => u.Id, u => u.Name);

                var result = transactions
                    .Select(t => new TransactionSummaryDto(
                        t.Id,
                        t.UserId,
                        userDict.TryGetValue(t.UserId, out var name) ? name : "Unknown",
                        t.MatchId,
                        t.OverallCreditChange,
                        t.Changes,
                        t.Status,
                        t.Type,
                        t.CreatedAt))
                    .OrderBy(t => t.UserName)
                    .ToList();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Completes a single pending transaction: applies credit change to the user
        /// and marks the transaction as Completed. Admin+.
        /// </summary>
        [HttpPost]
        [Route("CompleteTransaction/{transactionId}")]
        [RequireRole(UserRole.Admin, UserRole.SuperAdmin)]
        public async Task<IActionResult> CompleteTransaction(Guid transactionId)
        {
            try
            {
                var transaction = await _transactionService.GetTransactionById(transactionId);
                if (transaction == null)
                    return NotFound(new { message = $"Transaction {transactionId} not found." });

                if (transaction.Status == TransactionStatus.Completed)
                    return BadRequest(new { message = "Transaction is already completed." });

                var user = await _userService.GetUserById(transaction.UserId);
                if (user == null)
                    return NotFound(new { message = $"User {transaction.UserId} not found." });

                user.Credits += (float)transaction.OverallCreditChange;
                await _userService.UpdateUser(user);

                transaction.Status = TransactionStatus.Completed;
                await _transactionService.UpdateTransaction(transaction);

                await _userCacheService.RefreshCache();

                return Ok(new { message = $"Transaction completed. Credits updated for {user.Name}." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Completes all pending transactions for a match: applies credit changes to
        /// each user and marks all transactions as Completed. Admin+.
        /// </summary>
        [HttpPost]
        [Route("CompleteAllTransactions/{matchId}")]
        [RequireRole(UserRole.Admin, UserRole.SuperAdmin)]
        public async Task<IActionResult> CompleteAllTransactions(Guid matchId)
        {
            try
            {
                var allTransactions = await _transactionService.GetTransactionsByMatchId(matchId);
                var pending         = allTransactions.Where(t => t.Status == TransactionStatus.Pending).ToList();

                if (pending.Count == 0)
                    return Ok(new { message = "No pending transactions to complete.", count = 0 });

                var users    = await _userService.GetAllUsers();
                var userDict = users.ToDictionary(u => u.Id, u => u);

                int completed = 0;
                foreach (var tx in pending)
                {
                    if (!userDict.TryGetValue(tx.UserId, out var user)) continue;

                    user.Credits += (float)tx.OverallCreditChange;
                    await _userService.UpdateUser(user);

                    tx.Status = TransactionStatus.Completed;
                    await _transactionService.UpdateTransaction(tx);
                    completed++;
                }

                await _userCacheService.RefreshCache();

                return Ok(new { message = $"Completed {completed} transaction(s).", count = completed });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Reverts all completed transactions for a match: reverses credit changes,
        /// deletes transactions, clears finalStats from questions, clears matchSummary,
        /// and resets status to MatchCompleted. SuperAdmin only.
        /// WARNING: Not fully atomic. Use with caution and only when necessary.
        /// </summary>
        [HttpPost]
        [Route("RevertMatchTransactions/{matchId}")]
        [RequireRole(UserRole.SuperAdmin)]
        public async Task<IActionResult> RevertMatchTransactions(Guid matchId)
        {
            var failedOperations = new List<string>();
            
            try
            {
                // ── Validate match status ─────────────────────────────────────
                var matchStatus = await _matchStatusService.GetMatchStatusByMatchId(matchId);
                if (matchStatus == null)
                    return NotFound(new { message = $"Match status not found for match {matchId}" });

                if (matchStatus.Status != MatchStatus.Done && 
                    matchStatus.Status != MatchStatus.TransactionsSettled && 
                    matchStatus.Status != MatchStatus.BetsSettled)
                {
                    return BadRequest(new { message = $"Can only revert transactions for matches with status BetsSettled, TransactionsSettled, or Done. Current status: {matchStatus.Status}" });
                }

                // ── Get all transactions for this match ───────────────────────
                var allTransactions = await _transactionService.GetTransactionsByMatchId(matchId);
                var completedTransactions = allTransactions.Where(t => t.Status == TransactionStatus.Completed).ToList();
                
                if (completedTransactions.Count == 0 && allTransactions.Count == 0)
                    return Ok(new { message = "No transactions found for this match.", count = 0, creditsReverted = 0 });

                // ── Reverse credit changes for completed transactions ─────────
                var users = await _userService.GetAllUsers();
                var userDict = users.ToDictionary(u => u.Id, u => u);
                
                int reverted = 0;
                int skippedUsers = 0;
                double totalCreditsReverted = 0;
                
                foreach (var tx in completedTransactions)
                {
                    if (!userDict.TryGetValue(tx.UserId, out var user))
                    {
                        failedOperations.Add($"User {tx.UserId} not found - transaction {tx.Id} skipped");
                        skippedUsers++;
                        continue;
                    }

                    try
                    {
                        // Reverse the credit change
                        user.Credits -= (float)tx.OverallCreditChange;
                        
                        // Validate credits don't go negative (safety check)
                        if (user.Credits < 0)
                        {
                            failedOperations.Add($"WARNING: User {user.Name} credits would go negative ({user.Credits}). Continuing anyway.");
                        }
                        
                        await _userService.UpdateUser(user);
                        
                        totalCreditsReverted += Math.Abs(tx.OverallCreditChange);
                        reverted++;
                    }
                    catch (Exception ex)
                    {
                        failedOperations.Add($"Failed to revert credits for user {user.Name}: {ex.Message}");
                    }
                }

                // ── Delete all transactions (completed and pending) ───────────
                int deleted = 0;
                int failedDeletes = 0;
                
                foreach (var tx in allTransactions)
                {
                    try
                    {
                        var success = await _transactionService.DeleteTransaction(tx.Id, tx.UserId);
                        if (success)
                            deleted++;
                        else
                            failedDeletes++;
                    }
                    catch (Exception ex)
                    {
                        failedOperations.Add($"Failed to delete transaction {tx.Id}: {ex.Message}");
                        failedDeletes++;
                    }
                }

                // ── Clear finalStats from all questions ───────────────────────
                var questions = await _questionService.GetQuestionsByMatchId(matchId);
                int questionsCleared = 0;
                int failedQuestions = 0;
                
                foreach (var question in questions)
                {
                    if (question.FinalStats != null)
                    {
                        try
                        {
                            question.FinalStats = null;
                            await _questionService.UpdateQuestion(question);
                            questionsCleared++;
                        }
                        catch (Exception ex)
                        {
                            failedOperations.Add($"Failed to clear finalStats for question {question.Id}: {ex.Message}");
                            failedQuestions++;
                        }
                    }
                }

                // ── Clear matchSummary and leaderboard ────────────────────────
                try
                {
                    matchStatus.MatchSummary = null;
                    matchStatus.Leaderboard = null;
                    matchStatus.CompletedAt = null;
                    
                    // ── Reset status to MatchCompleted ────────────────────────────
                    matchStatus.Status = MatchStatus.MatchCompleted;
                    await _matchStatusService.UpdateMatchStatus(matchStatus);
                }
                catch (Exception ex)
                {
                    failedOperations.Add($"Failed to update match status: {ex.Message}");
                    return StatusCode(500, new { 
                        message = "CRITICAL: Failed to update match status. Database may be in inconsistent state.",
                        error = ex.Message,
                        warnings = failedOperations
                    });
                }

                // ── Refresh user cache ────────────────────────────────────────
                try
                {
                    await _userCacheService.RefreshCache();
                }
                catch (Exception ex)
                {
                    failedOperations.Add($"Failed to refresh user cache: {ex.Message}");
                }

                var message = $"Reverted {reverted} completed transaction(s), deleted {deleted} total transaction(s), cleared {questionsCleared} question finalStats, and reset match status to MatchCompleted.";
                
                if (failedOperations.Count > 0)
                {
                    message += $"\n\n⚠️ WARNINGS: {failedOperations.Count} operation(s) had issues.";
                }

                return Ok(new { 
                    message = message,
                    completedReverted = reverted,
                    totalDeleted = deleted,
                    failedDeletes = failedDeletes,
                    questionsCleared = questionsCleared,
                    failedQuestions = failedQuestions,
                    skippedUsers = skippedUsers,
                    totalCreditsReverted = Math.Round(totalCreditsReverted, 2),
                    warnings = failedOperations,
                    hasWarnings = failedOperations.Count > 0
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    message = $"CRITICAL ERROR: Revert operation failed. Database may be in inconsistent state: {ex.Message}",
                    warnings = failedOperations,
                    stackTrace = ex.StackTrace
                });
            }
        }
    }
}
