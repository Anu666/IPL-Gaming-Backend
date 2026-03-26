using IPL.Gaming.Common.Models.CosmosDB;
using IPL.Gaming.Repository.Interfaces;
using IPL.Gaming.Services.Interfaces;

namespace IPL.Gaming.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _transactionRepository;

        public TransactionService(ITransactionRepository transactionRepository)
        {
            _transactionRepository = transactionRepository;
        }

        public async Task<List<Transaction>> GetAllTransactions()
        {
            return await _transactionRepository.GetAllTransactions();
        }

        public async Task<Transaction> GetTransactionById(Guid transactionId)
        {
            return await _transactionRepository.GetTransactionById(transactionId);
        }

        public async Task<List<Transaction>> GetTransactionsByUserId(Guid userId)
        {
            return await _transactionRepository.GetTransactionsByUserId(userId);
        }

        public async Task<Transaction> GetTransactionByMatchAndUser(Guid matchId, Guid userId)
        {
            return await _transactionRepository.GetTransactionByMatchAndUser(matchId, userId);
        }

        public async Task<Transaction> CreateTransaction(Transaction transaction)
        {
            transaction.Id = Guid.NewGuid();
            return await _transactionRepository.CreateTransaction(transaction);
        }

        public async Task<Transaction> UpdateTransaction(Transaction transaction)
        {
            var existing = await _transactionRepository.GetTransactionById(transaction.Id);
            if (existing == null)
            {
                throw new Exception($"Transaction with ID {transaction.Id} not found");
            }

            return await _transactionRepository.UpdateTransaction(transaction);
        }

        public async Task<bool> DeleteTransaction(Guid transactionId, Guid userId)
        {
            return await _transactionRepository.DeleteTransaction(transactionId, userId);
        }
    }
}
