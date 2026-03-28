using IPL.Gaming.Common.Enums;
using IPL.Gaming.Common.Models.CosmosDB;
using IPL.Gaming.Services.Interfaces;

namespace IPL.Gaming.Services
{
    public class BetSettlementService : IBetSettlementService
    {
        private readonly IUserService _userService;
        private readonly IUserAnswerService _userAnswerService;
        private readonly IQuestionService _questionService;
        private readonly IMatchStatusService _matchStatusService;
        private readonly ITransactionService _transactionService;

        public BetSettlementService(
            IUserService userService,
            IUserAnswerService userAnswerService,
            IQuestionService questionService,
            IMatchStatusService matchStatusService,
            ITransactionService transactionService)
        {
            _userService = userService;
            _userAnswerService = userAnswerService;
            _questionService = questionService;
            _matchStatusService = matchStatusService;
            _transactionService = transactionService;
        }

        public async Task SettleBets(Guid matchId)
        {
            Console.WriteLine($"[BetSettlementService] Starting bet settlement for match {matchId}...");

            // ── Guard: match must be MatchCompleted ───────────────────────────
            var matchStatus = await _matchStatusService.GetMatchStatusByMatchId(matchId);
            if (matchStatus == null || matchStatus.Status != MatchStatus.MatchCompleted)
                throw new InvalidOperationException("Bets can only be settled when match status is MatchCompleted.");

            // ── Load questions and validate all have correct answers ──────────
            var questions = await _questionService.GetQuestionsByMatchId(matchId);
            if (questions.Count == 0)
                throw new InvalidOperationException("No questions found for this match.");

            var unansweredQuestions = questions.Where(q => q.CorrectOptionId == null).ToList();
            if (unansweredQuestions.Count > 0)
                throw new InvalidOperationException(
                    $"Correct answers not set for {unansweredQuestions.Count} question(s). Set all correct answers before settling.");

            // ── Build eligible player pool: all active users ──────────────────
            var allUsers = await _userService.GetAllUsers();
            var eligibleUsers = allUsers.Where(u => u.IsActive).ToList();
            var userDict = eligibleUsers.ToDictionary(u => u.Id, u => u.Name);
            int totalEligible = eligibleUsers.Count;

            Console.WriteLine($"[BetSettlementService] Eligible pool: {totalEligible} user(s).");

            // ── Build answer lookup: userId → (questionId → selectedOption) ───
            var userAnswers = await _userAnswerService.GetUserAnswersByMatchId(matchId);

            // userId → questionId → selectedOption
            var answerMap = new Dictionary<Guid, Dictionary<Guid, int>>();
            foreach (var ua in userAnswers)
            {
                if (!answerMap.ContainsKey(ua.UserId))
                    answerMap[ua.UserId] = new Dictionary<Guid, int>();

                foreach (var ans in ua.Answers)
                    answerMap[ua.UserId][ans.QuestionId] = ans.SelectedOption;
            }

            // ── Per-user credit accumulator: userId → List<Change> ────────────
            var userChanges = eligibleUsers.ToDictionary(
                u => u.Id,
                _ => new List<Change>());

            // ── Process each question ─────────────────────────────────────────
            foreach (var question in questions)
            {
                int correctOption = question.CorrectOptionId!.Value;

                // Classify every eligible user for this question
                var winners = new List<VoterInfo>();
                var losers  = new List<VoterInfo>();
                var autoLost = new List<VoterInfo>();

                foreach (var user in eligibleUsers)
                {
                    var name = userDict[user.Id];
                    int selected = 0;
                    bool answered = answerMap.TryGetValue(user.Id, out var qMap)
                                 && qMap.TryGetValue(question.Id, out selected);

                    if (!answered)
                    {
                        autoLost.Add(new VoterInfo { UserId = user.Id, UserName = name });
                    }
                    else if (selected == correctOption)
                    {
                        winners.Add(new VoterInfo { UserId = user.Id, UserName = name });
                    }
                    else
                    {
                        losers.Add(new VoterInfo { UserId = user.Id, UserName = name });
                    }
                }

                // Zero-winner → void
                bool isVoided = winners.Count == 0;
                double bonusPerWinner = 0;

                if (!isVoided)
                {
                    double loserPool = (losers.Count + autoLost.Count) * (double)question.Credits;
                    bonusPerWinner = Math.Round(loserPool / winners.Count, 2);
                }

                // Assign changes to each eligible user
                foreach (var w in winners)
                    userChanges[w.UserId].Add(new Change
                    {
                        QuestionId   = question.Id,
                        CreditChange = isVoided ? 0 : bonusPerWinner,
                        Outcome      = isVoided ? OutcomeType.Voided : OutcomeType.Won
                    });

                foreach (var l in losers)
                    userChanges[l.UserId].Add(new Change
                    {
                        QuestionId   = question.Id,
                        CreditChange = isVoided ? 0 : -(double)question.Credits,
                        Outcome      = isVoided ? OutcomeType.Voided : OutcomeType.Lost
                    });

                foreach (var al in autoLost)
                    userChanges[al.UserId].Add(new Change
                    {
                        QuestionId   = question.Id,
                        CreditChange = isVoided ? 0 : -(double)question.Credits,
                        Outcome      = isVoided ? OutcomeType.Voided : OutcomeType.AutoLost
                    });

                // Stamp FinalStats on question
                question.FinalStats = new QuestionFinalStats
                {
                    CorrectOptionId      = correctOption,
                    Winners              = winners,
                    Losers               = losers,
                    AutoLost             = autoLost,
                    IsVoided             = isVoided,
                    CreditChangePerWinner = bonusPerWinner,
                    SettledAt            = DateTime.UtcNow
                };

                await _questionService.UpdateQuestion(question);
                Console.WriteLine($"[BetSettlementService] Question '{question.QuestionText}': {winners.Count} winner(s), {losers.Count} loser(s), {autoLost.Count} auto-lost, voided={isVoided}.");
            }

            // ── Update IsCorrect on every UserAnswer ──────────────────────────
            var correctOptionByQuestion = questions.ToDictionary(q => q.Id, q => q.CorrectOptionId!.Value);

            foreach (var ua in userAnswers)
            {
                bool changed = false;
                foreach (var ans in ua.Answers)
                {
                    if (correctOptionByQuestion.TryGetValue(ans.QuestionId, out int correctOpt))
                    {
                        ans.IsCorrect = ans.SelectedOption == correctOpt;
                        changed = true;
                    }
                }
                if (changed)
                    await _userAnswerService.UpdateUserAnswer(ua);
            }

            Console.WriteLine("[BetSettlementService] UserAnswer.IsCorrect flags updated.");

            // ── Upsert one Transaction per eligible user ──────────────────────
            foreach (var user in eligibleUsers)
            {
                var changes = userChanges[user.Id];
                double overall = Math.Round(changes.Sum(c => c.CreditChange), 2);

                Transaction? existing = null;
                try
                {
                    existing = await _transactionService.GetTransactionByMatchAndUser(matchId, user.Id);
                }
                catch
                {
                    // Not found — will create below
                }

                if (existing != null)
                {
                    existing.Changes             = changes;
                    existing.OverallCreditChange = overall;
                    existing.Status              = TransactionStatus.Pending;
                    existing.Type                = TransactionType.MatchSettlement;
                    await _transactionService.UpdateTransaction(existing);
                }
                else
                {
                    await _transactionService.CreateTransaction(new Transaction
                    {
                        MatchId              = matchId,
                        UserId               = user.Id,
                        Changes              = changes,
                        OverallCreditChange  = overall,
                        Status               = TransactionStatus.Pending,
                        Type                 = TransactionType.MatchSettlement
                    });
                }
            }

            Console.WriteLine($"[BetSettlementService] Transactions written for {totalEligible} user(s).");

            // ── Build and stamp MatchSummary on the status record ─────────────
            matchStatus.MatchSummary = eligibleUsers
                .Select(u => new MatchSummaryEntry
                {
                    UserId              = u.Id,
                    UserName            = u.Name,
                    OverallCreditChange = Math.Round(userChanges[u.Id].Sum(c => c.CreditChange), 2),
                    Changes             = userChanges[u.Id]
                })
                .OrderBy(e => e.UserName)
                .ToList();

            // ── Transition to BetsSettled ─────────────────────────────────────
            matchStatus.Status = MatchStatus.BetsSettled;
            await _matchStatusService.UpdateMatchStatus(matchStatus);

            Console.WriteLine($"[BetSettlementService] Match {matchId} settled successfully → BetsSettled.");
        }
    }
}
