using IPL.Gaming.Common.Enums;
using IPL.Gaming.Common.Models.CosmosDB;
using IPL.Gaming.Services.Interfaces;

namespace IPL.Gaming.Services
{
    public class BettingStatsService : IBettingStatsService
    {
        private readonly IUserService _userService;
        private readonly IUserAnswerService _userAnswerService;
        private readonly IQuestionService _questionService;
        private readonly IMatchStatusService _matchStatusService;

        public BettingStatsService(
            IUserService userService,
            IUserAnswerService userAnswerService,
            IQuestionService questionService,
            IMatchStatusService matchStatusService)
        {
            _userService = userService;
            _userAnswerService = userAnswerService;
            _questionService = questionService;
            _matchStatusService = matchStatusService;
        }

        public async Task CalculateAndUpdateBettingStats(Guid matchId)
        {
            Console.WriteLine($"[BettingStatsService] Calculating betting stats for match {matchId}...");

            // 1. Build eligible player pool: isActive == true AND role == Player
            var allUsers = await _userService.GetAllUsers();
            var eligiblePlayers = allUsers
                .Where(u => u.IsActive)
                .ToList();

            var playerDict = eligiblePlayers.ToDictionary(u => u.Id, u => u.Name);
            int totalEligible = eligiblePlayers.Count;

            Console.WriteLine($"[BettingStatsService] Eligible player pool: {totalEligible}");

            // 2. Fetch all UserAnswers for this match
            var userAnswers = await _userAnswerService.GetUserAnswersByMatchId(matchId);

            // Build lookup: questionId → optionId → List<voterId>
            // Only count answers from eligible players
            var answersByQuestion = new Dictionary<Guid, Dictionary<int, List<Guid>>>();
            foreach (var ua in userAnswers)
            {
                if (!playerDict.ContainsKey(ua.UserId))
                    continue; // skip non-player or inactive users

                foreach (var answer in ua.Answers)
                {
                    if (!answersByQuestion.ContainsKey(answer.QuestionId))
                        answersByQuestion[answer.QuestionId] = new Dictionary<int, List<Guid>>();

                    var byOption = answersByQuestion[answer.QuestionId];
                    if (!byOption.ContainsKey(answer.SelectedOption))
                        byOption[answer.SelectedOption] = new List<Guid>();

                    byOption[answer.SelectedOption].Add(ua.UserId);
                }
            }

            // 3. Fetch all questions for this match
            var questions = await _questionService.GetQuestionsByMatchId(matchId);

            // 4. Compute and persist betting stats per question
            foreach (var question in questions)
            {
                var optionVotes = answersByQuestion.GetValueOrDefault(question.Id, new Dictionary<int, List<Guid>>());

                // Total eligible players who answered this specific question
                int totalVotes = optionVotes.Values.Sum(v => v.Count);
                int unansweredCount = totalEligible - totalVotes;

                var optionStats = new List<OptionBettingStats>();
                foreach (var option in question.Options)
                {
                    var voterIds = optionVotes.GetValueOrDefault(option.Id, new List<Guid>());
                    int voteCount = voterIds.Count;

                    // Losers = everyone who didn't pick this option (other-option voters + unanswered)
                    int losers = totalEligible - voteCount;
                    double loserPool = losers * (double)question.Credits;

                    // Bonus per winner; 0 if no one picked this option (avoid divide-by-zero)
                    double potentialWinCredits = voteCount == 0
                        ? 0
                        : Math.Round(loserPool / voteCount, 2);

                    optionStats.Add(new OptionBettingStats
                    {
                        OptionId = option.Id,
                        VoteCount = voteCount,
                        Voters = voterIds
                            .Select(uid => new VoterInfo { UserId = uid, UserName = playerDict[uid] })
                            .ToList(),
                        PotentialWinCredits = potentialWinCredits
                    });
                }

                question.BettingStats = new QuestionBettingStats
                {
                    TotalEligible = totalEligible,
                    TotalVotes = totalVotes,
                    UnansweredCount = unansweredCount,
                    OptionStats = optionStats,
                    LastCalculatedAt = DateTime.UtcNow
                };

                await _questionService.UpdateQuestion(question);
                Console.WriteLine($"[BettingStatsService] Updated stats for question {question.Id} ('{question.QuestionText}')");
            }

            Console.WriteLine($"[BettingStatsService] Done. Processed {questions.Count} question(s) for match {matchId}.");

            // Update match status to BetsUpdated
            var matchStatus = await _matchStatusService.GetMatchStatusByMatchId(matchId);
            if (matchStatus != null)
            {
                matchStatus.Status = MatchStatus.BetsUpdated;
                await _matchStatusService.UpdateMatchStatus(matchStatus);
                Console.WriteLine($"[BettingStatsService] Match {matchId} status updated to BetsUpdated.");
            }
            else
            {
                Console.WriteLine($"[BettingStatsService] Warning: No status record found for match {matchId} — skipping BetsUpdated transition.");
            }
        }
    }
}
