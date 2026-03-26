using IPL.Gaming.Common.Enums;
using Newtonsoft.Json;
using System;

namespace IPL.Gaming.Common.Models.CosmosDB
{
    public class User
    {
        [JsonProperty(PropertyName = "id")]
        public Guid Id { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; }

        [JsonProperty(PropertyName = "phoneNumber")]
        public string PhoneNumber { get; set; }

        [JsonProperty(PropertyName = "apiKey")]
        public string ApiKey { get; set; }

        [JsonProperty(PropertyName = "createdDate")]
        public DateTime CreatedDate { get; set; }

        [JsonProperty(PropertyName = "updatedDate")]
        public DateTime UpdatedDate { get; set; }

        [JsonProperty(PropertyName = "isActive")]
        public bool IsActive { get; set; }

        [JsonProperty(PropertyName = "lastLoginDate")]
        public DateTime? LastLoginDate { get; set; }

        [JsonProperty(PropertyName = "role")]
        public UserRole Role { get; set; }

        [JsonProperty(PropertyName = "credits")]
        public float Credits { get; set; }
    }
}
