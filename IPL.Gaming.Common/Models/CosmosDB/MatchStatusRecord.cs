using IPL.Gaming.Common.Enums;
using Newtonsoft.Json;

namespace IPL.Gaming.Common.Models.CosmosDB
{
    public class MatchStatusRecord
    {
        [JsonProperty("id")] public Guid Id { get; set; }
        [JsonProperty("matchId")] public Guid MatchId { get; set; }
        [JsonProperty("status")] public MatchStatus Status { get; set; }
        [JsonProperty("matchSummary")] public List<MatchSummaryEntry>? MatchSummary { get; set; }
        [JsonProperty("completedAt")] public DateTime? CompletedAt { get; set; }
        [JsonProperty("leaderboard")] public List<LeaderboardEntry>? Leaderboard { get; set; }
    }
}
