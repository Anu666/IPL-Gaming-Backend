using IPL.Gaming.Common.Models.CosmosDB;

namespace IPL.Gaming.Common.Models.Requests
{
    public class UpdateQuestionRequest
    {
        public Guid Id { get; set; }
        public Guid MatchId { get; set; }
        public string QuestionText { get; set; }
        public List<Option> Options { get; set; }
        public float Credits { get; set; }
        public int Sequence { get; set; }
        public int? CorrectOptionId { get; set; }
    }
}
