using Newtonsoft.Json;

namespace IPL.Gaming.Common.Models.CosmosDB
{
    public class VoterInfo
    {
        [JsonProperty("userId")]
        public Guid UserId { get; set; }

        [JsonProperty("userName")]
        public string UserName { get; set; }
    }
}
