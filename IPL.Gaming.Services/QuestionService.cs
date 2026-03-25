using IPL.Gaming.Common.Models.CosmosDB;
using IPL.Gaming.Repository.Interfaces;
using IPL.Gaming.Services.Interfaces;

namespace IPL.Gaming.Services
{
    public class QuestionService : IQuestionService
    {
        private readonly IQuestionRepository _questionRepository;

        public QuestionService(IQuestionRepository questionRepository)
        {
            _questionRepository = questionRepository;
        }

        public async Task<List<Question>> GetAllQuestions()
        {
            return await _questionRepository.GetAllQuestions();
        }

        public async Task<Question> GetQuestionById(Guid questionId)
        {
            return await _questionRepository.GetQuestionById(questionId);
        }

        public async Task<List<Question>> GetQuestionsByMatchId(Guid matchId)
        {
            return await _questionRepository.GetQuestionsByMatchId(matchId);
        }

        public async Task<Question> CreateQuestion(Question question)
        {
            question.Id = Guid.NewGuid();
            return await _questionRepository.CreateQuestion(question);
        }

        public async Task<Question> UpdateQuestion(Question question)
        {
            var existingQuestion = await _questionRepository.GetQuestionById(question.Id);
            if (existingQuestion == null)
            {
                throw new Exception($"Question with ID {question.Id} not found");
            }

            return await _questionRepository.UpdateQuestion(question);
        }

        public async Task<bool> DeleteQuestion(Guid questionId, Guid matchId)
        {
            return await _questionRepository.DeleteQuestion(questionId, matchId);
        }
    }
}
