using Newtonsoft.Json;

namespace IPL.Gaming.Common.Models.CosmosDB
{
    public class Option
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("optionText")]
        public string OptionText { get; set; }
    }
}
