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

            // Include all Done records (previous completed matches) + the current match record
            // (which is still TransactionsSettled at this point but has MatchSummary from SettleBets)
            var relevantRecords = allRecords
                .Where(r => r.MatchSummary != null && r.MatchSummary.Count > 0 &&
                            (r.Status == MatchStatus.Done || r.MatchId == currentMatchId))
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
