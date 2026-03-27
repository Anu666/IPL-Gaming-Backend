using Newtonsoft.Json;

namespace IPL.Gaming.Common.Models.CosmosDB
{
    public class QuestionBettingStats
    {
        /// <summary>Total active Player-role users — the full eligible pool.</summary>
        [JsonProperty("totalEligible")]
        public int TotalEligible { get; set; }

        /// <summary>How many eligible players actually answered this question.</summary>
        [JsonProperty("totalVotes")]
        public int TotalVotes { get; set; }

        /// <summary>TotalEligible - TotalVotes. These users auto-lose their credits.</summary>
        [JsonProperty("unansweredCount")]
        public int UnansweredCount { get; set; }

        [JsonProperty("optionStats")]
        public List<OptionBettingStats> OptionStats { get; set; }

        [JsonProperty("lastCalculatedAt")]
        public DateTime LastCalculatedAt { get; set; }
    }
}
