using IPL.Gaming.Common.Models.CosmosDB;
using IPL.Gaming.Database.Interfaces;
using IPL.Gaming.Repository.Interfaces;
using IPL.Gaming.Store;
using Microsoft.Azure.Cosmos;

namespace IPL.Gaming.Repository
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly ICosmosService _cosmosService;
        private readonly string containerName = DataStore.Transaction;

        public TransactionRepository(ICosmosService cosmosService)
        {
            _cosmosService = cosmosService;
        }

        public async Task<List<Transaction>> GetAllTransactions()
        {
            var queryDefinition = new QueryDefinition("SELECT * FROM T");
            var transactions = await _cosmosService.GetItemsAsync<Transaction>(containerName, queryDefinition);
            return transactions.ToList();
        }

        public async Task<Transaction> GetTransactionById(Guid transactionId)
        {
            var queryDefinition = new QueryDefinition("SELECT * FROM T WHERE T.id = @id")
                .WithParameter("@id", transactionId.ToString());
            var transactions = await _cosmosService.GetItemsAsync<Transaction>(containerName, queryDefinition);
            return transactions.FirstOrDefault();
        }

        public async Task<List<Transaction>> GetTransactionsByUserId(Guid userId)
        {
            var queryDefinition = new QueryDefinition("SELECT * FROM T WHERE T.userId = @userId")
                .WithParameter("@userId", userId.ToString());
            var transactions = await _cosmosService.GetItemsAsync<Transaction>(containerName, queryDefinition);
            return transactions.ToList();
        }

        public async Task<Transaction> GetTransactionByMatchAndUser(Guid matchId, Guid userId)
        {
            var queryDefinition = new QueryDefinition("SELECT * FROM T WHERE T.matchId = @matchId AND T.userId = @userId")
                .WithParameter("@matchId", matchId.ToString())
                .WithParameter("@userId", userId.ToString());
            var transactions = await _cosmosService.GetItemsAsync<Transaction>(containerName, queryDefinition);
            return transactions.FirstOrDefault();
        }

        public async Task<Transaction> CreateTransaction(Transaction transaction)
        {
            return await _cosmosService.AddItemAsync(containerName, transaction, transaction.UserId.ToString());
        }

        public async Task<Transaction> UpdateTransaction(Transaction transaction)
        {
            return await _cosmosService.UpsertItemAsync(containerName, transaction, transaction.UserId.ToString());
        }

        public async Task<bool> DeleteTransaction(Guid transactionId, Guid userId)
        {
            try
            {
                await _cosmosService.DeleteItemAsync<Transaction>(containerName, transactionId.ToString(), userId.ToString());
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
