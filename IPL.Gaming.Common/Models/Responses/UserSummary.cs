using IPL.Gaming.Common.Enums;
using Newtonsoft.Json;

namespace IPL.Gaming.Common.Models.Responses
{
    /// <summary>
    /// User details safe for Admin-level access — does not include ApiKey.
    /// </summary>
    public class UserSummary
    {
        [JsonProperty(PropertyName = "id")]
        public Guid Id { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; }

        [JsonProperty(PropertyName = "phoneNumber")]
        public string PhoneNumber { get; set; }

        [JsonProperty(PropertyName = "isActive")]
        public bool IsActive { get; set; }

        [JsonProperty(PropertyName = "role")]
        public UserRole Role { get; set; }

        [JsonProperty(PropertyName = "credits")]
        public float Credits { get; set; }

        [JsonProperty(PropertyName = "createdDate")]
        public DateTime CreatedDate { get; set; }

        [JsonProperty(PropertyName = "lastLoginDate")]
        public DateTime? LastLoginDate { get; set; }
    }
}
