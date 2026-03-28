using Newtonsoft.Json;

namespace IPL.Gaming.Common.Models.CosmosDB
{
    public class QuestionFinalStats
    {
        /// <summary>The option ID that was the correct answer.</summary>
        [JsonProperty("correctOptionId")]
        public int CorrectOptionId { get; set; }

        /// <summary>Eligible players who picked the correct option.</summary>
        [JsonProperty("winners")]
        public List<VoterInfo> Winners { get; set; } = new();

        /// <summary>Eligible players who picked a wrong option.</summary>
        [JsonProperty("losers")]
        public List<VoterInfo> Losers { get; set; } = new();

        /// <summary>Eligible players who did not answer this question at all.</summary>
        [JsonProperty("autoLost")]
        public List<VoterInfo> AutoLost { get; set; } = new();

        /// <summary>
        /// True when no eligible player picked the correct option.
        /// In that case creditChange is 0 for everyone (question is void).
        /// </summary>
        [JsonProperty("isVoided")]
        public bool IsVoided { get; set; }

        /// <summary>Bonus credits awarded to each winner (0 when voided).</summary>
        [JsonProperty("creditChangePerWinner")]
        public double CreditChangePerWinner { get; set; }

        [JsonProperty("settledAt")]
        public DateTime SettledAt { get; set; }
    }
}
