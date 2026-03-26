using IPL.Gaming.Common.Models.CosmosDB;
using IPL.Gaming.Common.Models.Requests;

namespace IPL.Gaming.Common.Mappers
{
    public static class UserMapper
    {
        /// <summary>
        /// Maps a CreateUserRequest to a new User object.
        /// Id, ApiKey, CreatedDate, UpdatedDate, and LastLoginDate are left at defaults
        /// and will be populated by the service layer.
        /// </summary>
        public static User ToUser(CreateUserRequest request)
        {
            return new User
            {
                Name = request.Name,
                Email = request.Email ?? string.Empty,
                PhoneNumber = request.PhoneNumber ?? string.Empty,
                Role = request.Role,
                Credits = request.Credits,
                IsActive = request.IsActive
            };
        }

        /// <summary>
        /// Applies an UpdateUserRequest onto an existing User, preserving
        /// ApiKey, Credits, CreatedDate, LastLoginDate, and Id.
        /// </summary>
        public static User ApplyUpdate(UpdateUserRequest request, User existingUser)
        {
            existingUser.Name = request.Name;
            existingUser.Email = request.Email ?? string.Empty;
            existingUser.PhoneNumber = request.PhoneNumber ?? string.Empty;
            existingUser.Role = request.Role;
            existingUser.IsActive = request.IsActive;

            // Preserved: existingUser.Id, ApiKey, Credits, CreatedDate, LastLoginDate
            return existingUser;
        }
    }
}
