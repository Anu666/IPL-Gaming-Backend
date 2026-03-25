using IPL.Gaming.Common.Models.CosmosDB;
using IPL.Gaming.Database.Interfaces;
using IPL.Gaming.Repository.Interfaces;
using IPL.Gaming.Store;
using Microsoft.Azure.Cosmos;

namespace IPL.Gaming.Repository
{
    public class UserAnswerRepository : IUserAnswerRepository
    {
        private readonly ICosmosService _cosmosService;
        private readonly string containerName = DataStore.UserAnswer;

        public UserAnswerRepository(ICosmosService cosmosService)
        {
            _cosmosService = cosmosService;
        }

        public async Task<List<UserAnswer>> GetAllUserAnswers()
        {
            var queryDefinition = new QueryDefinition("SELECT * FROM UA");
            var userAnswers = await _cosmosService.GetItemsAsync<UserAnswer>(containerName, queryDefinition);
            return userAnswers.ToList();
        }

        public async Task<UserAnswer> GetUserAnswerById(Guid userAnswerId)
        {
            var queryDefinition = new QueryDefinition("SELECT * FROM UA WHERE UA.id = @id")
                .WithParameter("@id", userAnswerId.ToString());
            var userAnswers = await _cosmosService.GetItemsAsync<UserAnswer>(containerName, queryDefinition);
            return userAnswers.FirstOrDefault();
        }

        public async Task<List<UserAnswer>> GetUserAnswersByMatchId(Guid matchId)
        {
            var queryDefinition = new QueryDefinition("SELECT * FROM UA WHERE UA.matchId = @matchId")
                .WithParameter("@matchId", matchId.ToString());
            var userAnswers = await _cosmosService.GetItemsAsync<UserAnswer>(containerName, queryDefinition);
            return userAnswers.ToList();
        }

        public async Task<UserAnswer> GetUserAnswerByMatchAndUser(Guid matchId, Guid userId)
        {
            var queryDefinition = new QueryDefinition("SELECT * FROM UA WHERE UA.matchId = @matchId AND UA.userId = @userId")
                .WithParameter("@matchId", matchId.ToString())
                .WithParameter("@userId", userId.ToString());
            var userAnswers = await _cosmosService.GetItemsAsync<UserAnswer>(containerName, queryDefinition);
            return userAnswers.FirstOrDefault();
        }

        public async Task<UserAnswer> CreateUserAnswer(UserAnswer userAnswer)
        {
            return await _cosmosService.AddItemAsync(containerName, userAnswer, userAnswer.MatchId.ToString());
        }

        public async Task<UserAnswer> UpdateUserAnswer(UserAnswer userAnswer)
        {
            return await _cosmosService.UpsertItemAsync(containerName, userAnswer, userAnswer.MatchId.ToString());
        }

        public async Task<bool> DeleteUserAnswer(Guid userAnswerId, Guid matchId)
        {
            try
            {
                await _cosmosService.DeleteItemAsync<UserAnswer>(containerName, userAnswerId.ToString(), matchId.ToString());
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
