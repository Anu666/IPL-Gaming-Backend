using IPL.Gaming.Common.Models.CosmosDB;
using IPL.Gaming.Common.Models.Requests;

namespace IPL.Gaming.Common.Mappers
{
    public static class QuestionMapper
    {
        public static Question ToQuestion(CreateQuestionRequest request) => new Question
        {
            MatchId = request.MatchId,
            QuestionText = request.QuestionText,
            Options = request.Options,
            Credits = request.Credits,
            Sequence = request.Sequence,
            CorrectOptionId = request.CorrectOptionId
        };

        public static Question ToQuestion(UpdateQuestionRequest request) => new Question
        {
            Id = request.Id,
            MatchId = request.MatchId,
            QuestionText = request.QuestionText,
            Options = request.Options,
            Credits = request.Credits,
            Sequence = request.Sequence,
            CorrectOptionId = request.CorrectOptionId
        };
    }
}
