using IPL.Gaming.Common.Models.CosmosDB;

namespace IPL.Gaming.Services.Interfaces
{
    public interface IQuestionService
    {
        Task<List<Question>> GetAllQuestions();
        Task<Question> GetQuestionById(Guid questionId);
        Task<List<Question>> GetQuestionsByMatchId(Guid matchId);
        Task<Question> CreateQuestion(Question question);
        Task<Question> UpdateQuestion(Question question);
        Task<bool> DeleteQuestion(Guid questionId, Guid matchId);
    }
}
