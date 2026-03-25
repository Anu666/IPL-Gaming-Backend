using IPL.Gaming.Common.Models.CosmosDB;
using IPL.Gaming.Repository.Interfaces;
using IPL.Gaming.Services.Interfaces;
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
    }
}
