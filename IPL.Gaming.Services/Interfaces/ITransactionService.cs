using IPL.Gaming.Common.Models.CosmosDB;

namespace IPL.Gaming.Services.Interfaces
{
    public interface ITransactionService
    {
        Task<List<Transaction>> GetAllTransactions();
        Task<Transaction> GetTransactionById(Guid transactionId);
        Task<List<Transaction>> GetTransactionsByUserId(Guid userId);
        Task<Transaction> GetTransactionByMatchAndUser(Guid matchId, Guid userId);
        Task<Transaction> CreateTransaction(Transaction transaction);
        Task<Transaction> UpdateTransaction(Transaction transaction);
        Task<bool> DeleteTransaction(Guid transactionId, Guid userId);
    }
}
