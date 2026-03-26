using IPL.Gaming.Common.Models.CosmosDB;

namespace IPL.Gaming.Common.Models.Requests
{
    public class CreateTransactionRequest
    {
        public Guid UserId { get; set; }
        public Guid MatchId { get; set; }
        public float OverallCreditChange { get; set; }
        public List<Change> Changes { get; set; }
    }
}
