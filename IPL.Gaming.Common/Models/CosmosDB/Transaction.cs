using IPL.Gaming.Common.Enums;
using Newtonsoft.Json;

namespace IPL.Gaming.Common.Models.CosmosDB
{
    public class Transaction
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }

        [JsonProperty("userId")]
        public Guid UserId { get; set; }

        [JsonProperty("matchId")]
        public Guid MatchId { get; set; }

        [JsonProperty("overallCreditChange")]
        public double OverallCreditChange { get; set; }

        [JsonProperty("changes")]
        public List<Change> Changes { get; set; }

        [JsonProperty("status")]
        public TransactionStatus Status { get; set; } = TransactionStatus.Pending;
    }
}
