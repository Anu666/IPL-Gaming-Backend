using IPL.Gaming.Common.Models.CosmosDB;
using IPL.Gaming.Common.Models.Requests;

namespace IPL.Gaming.Common.Mappers
{
    public static class TransactionMapper
    {
        public static Transaction ToTransaction(CreateTransactionRequest request) => new Transaction
        {
            UserId = request.UserId,
            MatchId = request.MatchId,
            OverallCreditChange = request.OverallCreditChange,
            Changes = request.Changes
        };

        public static Transaction ToTransaction(UpdateTransactionRequest request) => new Transaction
        {
            Id = request.Id,
            UserId = request.UserId,
            MatchId = request.MatchId,
            OverallCreditChange = request.OverallCreditChange,
            Changes = request.Changes
        };
    }
}
