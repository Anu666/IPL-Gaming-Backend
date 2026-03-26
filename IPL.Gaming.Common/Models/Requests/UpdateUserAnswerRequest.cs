using IPL.Gaming.Common.Models.CosmosDB;

namespace IPL.Gaming.Common.Models.Requests
{
    public class UpdateUserAnswerRequest
    {
        public Guid Id { get; set; }
        public Guid MatchId { get; set; }
        public Guid UserId { get; set; }
        public List<Answer> Answers { get; set; }
    }
}
