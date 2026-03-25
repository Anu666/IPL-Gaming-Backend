using Newtonsoft.Json;

namespace IPL.Gaming.Common.Models.CosmosDB
{
    public class UserAnswer
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }

        [JsonProperty("matchId")]
        public Guid MatchId { get; set; }

        [JsonProperty("userId")]
        public Guid UserId { get; set; }

        [JsonProperty("answers")]
        public List<Answer> Answers { get; set; }
    }
}
