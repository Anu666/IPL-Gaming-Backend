using IPL.Gaming.Common.Enums;
using IPL.Gaming.Common.Models.CosmosDB;
using IPL.Gaming.Repository.Interfaces;
using IPL.Gaming.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IPL.Gaming.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<List<User>> GetAllUsers()
        {
            return await _userRepository.GetAllUsers();
        }

        public async Task<User> GetUserById(Guid userId)
        {
            return await _userRepository.GetUserById(userId);
        }

        public async Task<User> GetUserByApiKey(string apiKey)
        {
            return await _userRepository.GetUserByApiKey(apiKey);
        }

        public async Task<User> CreateUser(User user)
        {
            // Always generate new ID (ignore any provided ID)
            user.Id = Guid.NewGuid();

            // Generate API Key
            user.ApiKey = Guid.NewGuid().ToString();

            // Set audit fields
            user.CreatedDate = DateTime.UtcNow;
            user.UpdatedDate = DateTime.UtcNow;

            // Set default values
            user.IsActive = true;
            user.Role = user.Role == 0 ? UserRole.Player : user.Role;

            return await _userRepository.CreateUser(user);
        }

        public async Task<User> UpdateUser(User user)
        {
            // Retrieve existing user
            var existingUser = await _userRepository.GetUserById(user.Id);
            if (existingUser == null)
            {
                throw new Exception($"User with ID {user.Id} not found");
            }

            // Update audit fields
            user.UpdatedDate = DateTime.UtcNow;

            // Preserve created date and API key from existing user
            user.CreatedDate = existingUser.CreatedDate;
            if (string.IsNullOrEmpty(user.ApiKey))
            {
                user.ApiKey = existingUser.ApiKey;
            }

            return await _userRepository.UpdateUser(user);
        }

        public async Task<bool> DeleteUser(Guid userId)
        {
            return await _userRepository.DeleteUser(userId);
        }
    }
}
