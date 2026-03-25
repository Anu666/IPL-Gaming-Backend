using IPL.Gaming.Common.Models.CosmosDB;
using IPL.Gaming.Repository.Interfaces;
using IPL.Gaming.Services.Interfaces;

namespace IPL.Gaming.Services
{
    public class UserAnswerService : IUserAnswerService
    {
        private readonly IUserAnswerRepository _userAnswerRepository;

        public UserAnswerService(IUserAnswerRepository userAnswerRepository)
        {
            _userAnswerRepository = userAnswerRepository;
        }

        public async Task<List<UserAnswer>> GetAllUserAnswers()
        {
            return await _userAnswerRepository.GetAllUserAnswers();
        }

        public async Task<UserAnswer> GetUserAnswerById(Guid userAnswerId)
        {
            return await _userAnswerRepository.GetUserAnswerById(userAnswerId);
        }

        public async Task<List<UserAnswer>> GetUserAnswersByMatchId(Guid matchId)
        {
            return await _userAnswerRepository.GetUserAnswersByMatchId(matchId);
        }

        public async Task<UserAnswer> GetUserAnswerByMatchAndUser(Guid matchId, Guid userId)
        {
            return await _userAnswerRepository.GetUserAnswerByMatchAndUser(matchId, userId);
        }

        public async Task<UserAnswer> CreateUserAnswer(UserAnswer userAnswer)
        {
            userAnswer.Id = Guid.NewGuid();
            return await _userAnswerRepository.CreateUserAnswer(userAnswer);
        }

        public async Task<UserAnswer> UpdateUserAnswer(UserAnswer userAnswer)
        {
            var existing = await _userAnswerRepository.GetUserAnswerById(userAnswer.Id);
            if (existing == null)
            {
                throw new Exception($"UserAnswer with ID {userAnswer.Id} not found");
            }

            return await _userAnswerRepository.UpdateUserAnswer(userAnswer);
        }

        public async Task<bool> DeleteUserAnswer(Guid userAnswerId, Guid matchId)
        {
            return await _userAnswerRepository.DeleteUserAnswer(userAnswerId, matchId);
        }
    }
}
