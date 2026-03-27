using IPL.Gaming.Common.Models.CosmosDB;
using IPL.Gaming.Database.Interfaces;
using IPL.Gaming.Repository.Interfaces;
using IPL.Gaming.Store;
using Microsoft.Azure.Cosmos;

namespace IPL.Gaming.Repository
{
    public class MatchStatusRepository : IMatchStatusRepository
    {
        private readonly ICosmosService _cosmosService;
        private readonly string containerName = DataStore.MatchStatus;

        public MatchStatusRepository(ICosmosService cosmosService)
        {
            _cosmosService = cosmosService;
        }

        public async Task<List<MatchStatusRecord>> GetAllMatchStatuses()
        {
            var queryDefinition = new QueryDefinition("SELECT * FROM MS");
            var results = await _cosmosService.GetItemsAsync<MatchStatusRecord>(containerName, queryDefinition);
            return results.ToList();
        }

        public async Task<MatchStatusRecord> GetMatchStatusById(Guid matchStatusId)
        {
            var queryDefinition = new QueryDefinition("SELECT * FROM MS WHERE MS.id = @id")
                .WithParameter("@id", matchStatusId.ToString());
            var results = await _cosmosService.GetItemsAsync<MatchStatusRecord>(containerName, queryDefinition);
            return results.FirstOrDefault();
        }

        public async Task<MatchStatusRecord> GetMatchStatusByMatchId(Guid matchId)
        {
            var queryDefinition = new QueryDefinition("SELECT * FROM MS WHERE MS.matchId = @matchId")
                .WithParameter("@matchId", matchId.ToString());
            var results = await _cosmosService.GetItemsAsync<MatchStatusRecord>(containerName, queryDefinition);
            return results.FirstOrDefault();
        }

        public async Task<MatchStatusRecord> CreateMatchStatus(MatchStatusRecord matchStatus)
        {
            return await _cosmosService.AddItemAsync(containerName, matchStatus, matchStatus.MatchId.ToString());
        }

        public async Task<MatchStatusRecord> UpdateMatchStatus(MatchStatusRecord matchStatus)
        {
            return await _cosmosService.UpsertItemAsync(containerName, matchStatus, matchStatus.MatchId.ToString());
        }

        public async Task<bool> DeleteMatchStatus(Guid matchStatusId, Guid matchId)
        {
            try
            {
                await _cosmosService.DeleteItemAsync<MatchStatusRecord>(containerName, matchStatusId.ToString(), matchId.ToString());
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
