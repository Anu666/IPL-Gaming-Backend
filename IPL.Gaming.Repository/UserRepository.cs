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
    }
}
