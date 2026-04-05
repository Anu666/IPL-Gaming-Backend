using IPL.Gaming.Common.Enums;
using IPL.Gaming.Common.Models.CosmosDB;
using IPL.Gaming.Repository.Interfaces;
using IPL.Gaming.Services.Interfaces;

namespace IPL.Gaming.Services
{
    public class LeaderboardService : ILeaderboardService
    {
        private readonly IMatchStatusRepository _matchStatusRepository;

        public LeaderboardService(IMatchStatusRepository matchStatusRepository)
        {
            _matchStatusRepository = matchStatusRepository;
        }

        public async Task<List<LeaderboardEntry>> CalculateCumulativeLeaderboard(Guid currentMatchId)
        {
            // All match status records
            var allRecords = await _matchStatusRepository.GetAllMatchStatuses();

            // Get the current match's completion time
            var currentMatch = allRecords.FirstOrDefault(r => r.MatchId == currentMatchId);
            if (currentMatch == null || currentMatch.CompletedAt == null)
                throw new InvalidOperationException($"Current match {currentMatchId} not found or has no completion time");

            // Include all Done/Archived matches that were completed AT OR BEFORE the current match
            // Using <= to include the current match in its own leaderboard
            var relevantRecords = allRecords
                .Where(r => r.MatchSummary != null && r.MatchSummary.Count > 0 &&
                            r.CompletedAt.HasValue &&
                            r.CompletedAt.Value <= currentMatch.CompletedAt.Value &&
                            (r.Status == MatchStatus.Done || r.Status == MatchStatus.Archived))
                .ToList();

            // Aggregate per user across all relevant records
            var aggregates = new Dictionary<Guid, LeaderboardEntry>();

            foreach (var record in relevantRecords)
            {
                foreach (var entry in record.MatchSummary!)
                {
                    if (!aggregates.TryGetValue(entry.UserId, out var agg))
                    {
                        agg = new LeaderboardEntry
                        {
                            UserId   = entry.UserId,
                            UserName = entry.UserName,
                        };
                        aggregates[entry.UserId] = agg;
                    }

                    agg.MatchesPlayed++;
                    agg.TotalCreditChange = Math.Round(agg.TotalCreditChange + entry.OverallCreditChange, 2);

                    foreach (var change in entry.Changes)
                    {
                        switch (change.Outcome)
                        {
                            case OutcomeType.Won:      agg.CorrectPredictions++;   break;
                            case OutcomeType.Lost:     agg.WrongPredictions++;     break;
                            case OutcomeType.AutoLost: agg.UnansweredQuestions++;  break;
                            case OutcomeType.Voided:   agg.VoidedQuestions++;      break;
                        }
                    }
                }
            }

            // Calculate win rate and assign rank
            var sorted = aggregates.Values
                .OrderByDescending(e => e.TotalCreditChange)
                .ThenByDescending(e => e.CorrectPredictions)
                .ToList();

            for (int i = 0; i < sorted.Count; i++)
            {
                var e = sorted[i];
                int total = e.CorrectPredictions + e.WrongPredictions;
                e.WinRate = total > 0
                    ? Math.Round((double)e.CorrectPredictions / total * 100, 1)
                    : 0;
                e.Rank = i + 1;
            }

            return sorted;
        }
    }
}
