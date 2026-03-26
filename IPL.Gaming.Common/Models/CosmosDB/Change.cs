using Newtonsoft.Json;

namespace IPL.Gaming.Common.Models.CosmosDB
{
    public class Change
    {
        [JsonProperty("questionId")]
        public Guid QuestionId { get; set; }

        [JsonProperty("creditChange")]
        public float CreditChange { get; set; }
    }
}
