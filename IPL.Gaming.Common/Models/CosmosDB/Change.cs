using IPL.Gaming.Common.Enums;
using Newtonsoft.Json;

namespace IPL.Gaming.Common.Models.CosmosDB
{
    public class Change
    {
        [JsonProperty("questionId")]
        public Guid QuestionId { get; set; }

        [JsonProperty("creditChange")]
        public double CreditChange { get; set; }

        [JsonProperty("outcome")]
        public OutcomeType Outcome { get; set; }
    }
}
