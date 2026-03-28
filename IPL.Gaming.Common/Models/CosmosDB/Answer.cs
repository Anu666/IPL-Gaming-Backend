using Newtonsoft.Json;

namespace IPL.Gaming.Common.Models.CosmosDB
{
    public class Answer
    {
        [JsonProperty("questionId")]
        public Guid QuestionId { get; set; }

        [JsonProperty("selectedOption")]
        public int SelectedOption { get; set; }

        /// <summary>Set during bet settlement. Null before settlement.</summary>
        [JsonProperty("isCorrect")]
        public bool? IsCorrect { get; set; }
    }
}
