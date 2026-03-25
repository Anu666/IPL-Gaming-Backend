using IPL.Gaming.Common.Enums;
using Newtonsoft.Json;
using System;

namespace IPL.Gaming.Common.Models.Repository
{
    public class MasterData<T>
    {
        [JsonProperty(PropertyName = "id")]
        public Guid Id { get; set; }
        [JsonProperty(PropertyName = "type")]
        public MasterDataType Type { get; set; }
        public T Data { get; set; }
    }
}
