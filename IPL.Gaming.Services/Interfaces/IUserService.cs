using IPL.Gaming.Common.Models.CosmosDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPL.Gaming.Services.Interfaces
{
    public interface IUserService
    {
        Task<List<User>> GetAllUsers();
        Task<User> GetUserById(Guid userId);
        Task<User> GetUserByApiKey(string apiKey);
        Task<User> CreateUser(User user);
        Task<User> UpdateUser(User user);
        Task<bool> DeleteUser(Guid userId);
    }
}
