using IPL.Gaming.Database.Interfaces;
using IPL.Gaming.Repository.Interfaces;
using IPL.Gaming.Store;
using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using User = IPL.Gaming.Common.Models.CosmosDB.User;

namespace IPL.Gaming.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ICosmosService _cosmosService;
        private readonly string containerName = DataStore.User;

        public UserRepository(ICosmosService cosmosService)
        {
            _cosmosService = cosmosService;
        }

        public async Task<List<User>> GetAllUsers()
        {
            var queryString = "SELECT * FROM U";
            var queryDefinition = new QueryDefinition(queryString);

            var users = await _cosmosService.GetItemsAsync<User>(containerName, queryDefinition);
            return users.ToList();
        }

        public async Task<User> GetUserById(Guid userId)
        {
            var queryString = "SELECT * FROM U WHERE U.id = @userId";
            var queryDefinition = new QueryDefinition(queryString)
                .WithParameter("@userId", userId.ToString());

            var users = await _cosmosService.GetItemsAsync<User>(containerName, queryDefinition);
            return users.FirstOrDefault();
        }

        public async Task<User> GetUserByApiKey(string apiKey)
        {
            var queryString = "SELECT * FROM U WHERE U.apiKey = @apiKey AND U.isActive = true";
            var queryDefinition = new QueryDefinition(queryString)
                .WithParameter("@apiKey", apiKey);

            var users = await _cosmosService.GetItemsAsync<User>(containerName, queryDefinition);
            return users.FirstOrDefault();
        }

        public async Task<User> CreateUser(User user)
        {
            var createdUser = await _cosmosService.AddItemAsync(containerName, user);
            return createdUser;
        }

        public async Task<User> UpdateUser(User user)
        {
            var updatedUser = await _cosmosService.UpsertItemAsync(containerName, user, user.Id.ToString());
            return updatedUser;
        }

        public async Task<bool> DeleteUser(Guid userId)
        {
            try
            {
                var userIdString = userId.ToString();
                await _cosmosService.DeleteItemAsync<User>(containerName, userIdString, userIdString);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
