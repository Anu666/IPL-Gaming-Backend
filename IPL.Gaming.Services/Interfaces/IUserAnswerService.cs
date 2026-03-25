using IPL.Gaming.Common.Models.CosmosDB;

namespace IPL.Gaming.Services.Interfaces
{
    public interface IUserAnswerService
    {
        Task<List<UserAnswer>> GetAllUserAnswers();
        Task<UserAnswer> GetUserAnswerById(Guid userAnswerId);
        Task<List<UserAnswer>> GetUserAnswersByMatchId(Guid matchId);
        Task<UserAnswer> GetUserAnswerByMatchAndUser(Guid matchId, Guid userId);
        Task<UserAnswer> CreateUserAnswer(UserAnswer userAnswer);
        Task<UserAnswer> UpdateUserAnswer(UserAnswer userAnswer);
        Task<bool> DeleteUserAnswer(Guid userAnswerId, Guid matchId);
    }
}
