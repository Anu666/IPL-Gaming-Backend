using IPL.Gaming.Common.Models.CosmosDB;
using IPL.Gaming.Database.Interfaces;
using IPL.Gaming.Repository.Interfaces;
using IPL.Gaming.Store;
using Microsoft.Azure.Cosmos;

namespace IPL.Gaming.Repository
{
    public class QuestionRepository : IQuestionRepository
    {
        private readonly ICosmosService _cosmosService;
        private readonly string containerName = DataStore.Question;

        public QuestionRepository(ICosmosService cosmosService)
        {
            _cosmosService = cosmosService;
        }

        public async Task<List<Question>> GetAllQuestions()
        {
            var queryString = "SELECT * FROM Q";
            var queryDefinition = new QueryDefinition(queryString);

            var questions = await _cosmosService.GetItemsAsync<Question>(containerName, queryDefinition);
            return questions.ToList();
        }

        public async Task<Question> GetQuestionById(Guid questionId)
        {
            var queryString = "SELECT * FROM Q WHERE Q.id = @questionId";
            var queryDefinition = new QueryDefinition(queryString)
                .WithParameter("@questionId", questionId.ToString());

            var questions = await _cosmosService.GetItemsAsync<Question>(containerName, queryDefinition);
            return questions.FirstOrDefault();
        }

        public async Task<List<Question>> GetQuestionsByMatchId(Guid matchId)
        {
            var queryString = "SELECT * FROM Q WHERE Q.matchId = @matchId";
            var queryDefinition = new QueryDefinition(queryString)
                .WithParameter("@matchId", matchId.ToString());

            var questions = await _cosmosService.GetItemsAsync<Question>(containerName, queryDefinition);
            return questions.ToList();
        }

        public async Task<Question> CreateQuestion(Question question)
        {
            var createdQuestion = await _cosmosService.AddItemAsync(containerName, question, question.MatchId.ToString());
            return createdQuestion;
        }

        public async Task<Question> UpdateQuestion(Question question)
        {
            var updatedQuestion = await _cosmosService.UpsertItemAsync(containerName, question, question.MatchId.ToString());
            return updatedQuestion;
        }

        public async Task<bool> DeleteQuestion(Guid questionId, Guid matchId)
        {
            try
            {
                await _cosmosService.DeleteItemAsync<Question>(containerName, questionId.ToString(), matchId.ToString());
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
