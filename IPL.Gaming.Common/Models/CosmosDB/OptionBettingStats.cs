using Newtonsoft.Json;

namespace IPL.Gaming.Common.Models.CosmosDB
{
    public class OptionBettingStats
    {
        [JsonProperty("optionId")]
        public int OptionId { get; set; }

        [JsonProperty("voteCount")]
        public int VoteCount { get; set; }

        [JsonProperty("voters")]
        public List<VoterInfo> Voters { get; set; }

        /// <summary>
        /// Bonus credits per winner if this option turns out to be correct.
        /// Does NOT include the original bet amount.
        /// Calculated as: (totalEligible - voteCount) × questionCredits / voteCount
        /// </summary>
        [JsonProperty("potentialWinCredits")]
        public double PotentialWinCredits { get; set; }
    }
}
