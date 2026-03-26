using IPL.Gaming.Common.Enums;

namespace IPL.Gaming.Common.Models.Requests
{
    public class CreateUserRequest
    {
        public string Name { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public UserRole Role { get; set; } = UserRole.Player;
        public float Credits { get; set; } = 0;
        public bool IsActive { get; set; } = true;
    }
}
