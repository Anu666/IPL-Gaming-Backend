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

        public BetSettlementController(
            IBetSettlementService betSettlementService,
            ITransactionService transactionService,
            IUserService userService,
            UserCacheService userCacheService)
        {
            _betSettlementService = betSettlementService;
            _transactionService   = transactionService;
            _userService          = userService;
            _userCacheService     = userCacheService;
        }

        // ── Inner DTO ─────────────────────────────────────────────────────────
        private record TransactionSummaryDto(
            Guid Id,
            Guid UserId,
            string UserName,
            Guid MatchId,
            double OverallCreditChange,
            List<Change> Changes,
            TransactionStatus Status);

        /// <summary>
        /// Settles all bets for a match. Requires status = MatchCompleted and all
        /// questions to have correctOptionId set. SuperAdmin only.
        /// </summary>
        [HttpPost]
        [Route("SettleBets/{matchId}")]
        [RequireRole(UserRole.SuperAdmin)]
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
                        t.Status))
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
        /// and marks the transaction as Completed. SuperAdmin only.
        /// </summary>
        [HttpPost]
        [Route("CompleteTransaction/{transactionId}")]
        [RequireRole(UserRole.SuperAdmin)]
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
        /// each user and marks all transactions as Completed. SuperAdmin only.
        /// </summary>
        [HttpPost]
        [Route("CompleteAllTransactions/{matchId}")]
        [RequireRole(UserRole.SuperAdmin)]
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
    }
}
