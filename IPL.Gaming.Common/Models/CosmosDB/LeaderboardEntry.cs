using Newtonsoft.Json;

namespace IPL.Gaming.Common.Models.CosmosDB
{
    public class LeaderboardEntry
    {
        [JsonProperty("userId")] public Guid UserId { get; set; }
        [JsonProperty("userName")] public string UserName { get; set; } = string.Empty;
        [JsonProperty("totalCreditChange")] public double TotalCreditChange { get; set; }
        [JsonProperty("correctPredictions")] public int CorrectPredictions { get; set; }
        [JsonProperty("wrongPredictions")] public int WrongPredictions { get; set; }
        [JsonProperty("unansweredQuestions")] public int UnansweredQuestions { get; set; }
        [JsonProperty("voidedQuestions")] public int VoidedQuestions { get; set; }
        [JsonProperty("matchesPlayed")] public int MatchesPlayed { get; set; }
        [JsonProperty("winRate")] public double WinRate { get; set; }
        [JsonProperty("rank")] public int Rank { get; set; }
    }
}
