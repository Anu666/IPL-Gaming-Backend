using IPL.Gaming.Common.Models.CosmosDB;
using IPL.Gaming.Common.Models.Requests;

namespace IPL.Gaming.Common.Mappers
{
    public static class UserAnswerMapper
    {
        public static UserAnswer ToUserAnswer(CreateUserAnswerRequest request) => new UserAnswer
        {
            MatchId = request.MatchId,
            UserId = request.UserId,
            Answers = request.Answers
        };

        public static UserAnswer ToUserAnswer(UpdateUserAnswerRequest request) => new UserAnswer
        {
            Id = request.Id,
            MatchId = request.MatchId,
            UserId = request.UserId,
            Answers = request.Answers
        };
    }
}
