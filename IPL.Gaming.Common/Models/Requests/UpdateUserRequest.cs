using IPL.Gaming.Common.Enums;

namespace IPL.Gaming.Common.Models.Requests
{
    public class UpdateUserRequest
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public UserRole Role { get; set; }
        public bool IsActive { get; set; }
        // Credits updated via dedicated PATCH /credits endpoint
        // ApiKey, CreatedDate are preserved from the existing record
    }
}
