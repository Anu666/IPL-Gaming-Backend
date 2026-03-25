using Newtonsoft.Json;

namespace IPL.Gaming.Common.Models.CosmosDB
{
    public class Question
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }

        [JsonProperty("matchId")]
        public Guid MatchId { get; set; }

        [JsonProperty("questionText")]
        public string QuestionText { get; set; }

        [JsonProperty("options")]
        public List<Option> Options { get; set; }

        [JsonProperty("credits")]
        public float Credits { get; set; }

        [JsonProperty("sequence")]
        public int Sequence { get; set; }

        [JsonProperty("correctOptionId")]
        public int? CorrectOptionId { get; set; }
    }
}
