using Newtonsoft.Json;

namespace IPL.Gaming.Common.Models.CosmosDB
{
    public class MatchSummaryEntry
    {
        [JsonProperty("userId")] public Guid UserId { get; set; }
        [JsonProperty("userName")] public string UserName { get; set; } = string.Empty;
        [JsonProperty("overallCreditChange")] public double OverallCreditChange { get; set; }
        [JsonProperty("changes")] public List<Change> Changes { get; set; } = new();
    }
}
