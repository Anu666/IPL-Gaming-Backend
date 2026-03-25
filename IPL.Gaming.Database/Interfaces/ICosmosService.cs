namespace IPL.Gaming.Database.Interfaces
{
    using Microsoft.Azure.Cosmos;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface ICosmosService
    {
        /// <summary>
        /// Adds the <paramref name="item"/> asynchronously.
        /// </summary>
        /// <typeparam name="T">The item type of model.</typeparam>
        /// <param name="containerName">Name of the container.</param>
        /// <param name="item">The item.</param>
        /// <param name="partitionId">The partition identifier.</param>
        /// <returns>The added item.</returns>
        Task<T> AddItemAsync<T>(string containerName, T item, string partitionId = null);

        /// <summary>
        /// Adds the <paramref name="item"/> asynchronously.
        /// </summary>
        /// <typeparam name="T">The item type of model.</typeparam>
        /// <param name="containerName">Name of the container.</param>
        /// <param name="item">The item.</param>
        /// <param name="partitionId">The partition identifier.</param>
        /// <returns>The added item.</returns>
        Task<T> AddItemAsync<T>(string containerName, T item, int partitionId);

        /// <summary>
        /// Upserts the <paramref name="item"/> asynchronously.
        /// </summary>
        /// <typeparam name="T">The item type of model.</typeparam>
        /// <param name="containerName">Name of the container.</param>
        /// <param name="item">The item.</param>
        /// <param name="partitionId">The partition identifier.</param>
        /// <returns>The updated item.</returns>
        Task<T> UpsertItemAsync<T>(string containerName, T item, string partitionId = null);

		/// <summary>
		/// Upserts the <paramref name="item"/> asynchronously.
		/// </summary>
		/// <typeparam name="T">The item type of model.</typeparam>
		/// <param name="containerName">Name of the container.</param>
		/// <param name="item">The item.</param>
		/// <param name="partitionId">The partition identifier.</param>
		/// <returns>The updated item.</returns>
		Task<T> UpsertItemAsync<T>(string containerName, T item, int partitionId);

		/// <summary>
		/// Deletes the item based on the <paramref name="id"/> asynchronously.
		/// </summary>
		/// <typeparam name="T">The item type of model.</typeparam>
		/// <param name="containerName">Name of the container.</param>
		/// <param name="id">The identifier.</param>
		/// <param name="partitionId">The partition identifier.</param>
		/// <returns>The deleted item.</returns>
		Task<T> DeleteItemAsync<T>(string containerName, string id, string partitionId);

        /// <summary>
        /// Deletes the items based on the <paramref name="containerName"/>, <paramref name="query"/> and <paramref name="partitionId"/> asynchronously.
        /// </summary>
        /// <typeparam name="T">The item type of model.</typeparam>
        /// <param name="containerName">Name of the container.</param>
        /// <param name="query">The query.</param>
        /// <param name="partitionId">The partition identifier.</param>
        /// <returns>Success or not.</returns>
        Task<bool> DeleteItemsByQueryAsync(string containerName, string query, string partitionId);

        /// <summary>
        /// Deletes the items based on the <paramref name="containerName"/>, <paramref name="query"/> and <paramref name="partitionId"/> asynchronously.
        /// </summary>
        /// <typeparam name="T">The item type of model.</typeparam>
        /// <param name="containerName">Name of the container.</param>
        /// <param name="query">The query.</param>
        /// <param name="partitionId">The partition identifier.</param>
        /// <returns>Success or not.</returns>
        Task<bool> DeleteItemsByQueryAsync(string containerName, string query, int partitionId);

        /// <summary>
        /// Gets the item based on the <paramref name="id"/> asynchronously.
        /// </summary>
        /// <typeparam name="T">The item type of model.</typeparam>
        /// <param name="containerName">Name of the container.</param>
        /// <param name="id">The identifier.</param>
        /// <param name="partitionId">The partition identifier.</param>
        /// <returns>The item.</returns>
        Task<T> GetItemAsync<T>(string containerName, string id, string partitionId);

		/// <summary>
		/// Gets the item based on the <paramref name="id"/> asynchronously.
		/// </summary>
		/// <typeparam name="T">The item type of model.</typeparam>
		/// <param name="containerName">Name of the container.</param>
		/// <param name="id">The identifier.</param>
		/// <param name="idPropertyName">The property of identifier in the container.</param>
		/// <returns>The item.</returns>
		Task<T> GetItemByIdAsync<T>(string containerName, string id, string idPropertyName = "id");

        Task<IEnumerable<T>> GetItemsAsync<T>(string databaseName, string containerName, QueryDefinition queryDefinition, string partitionId = null);

		/// <summary>
		/// Gets the items asynchronously based on the <paramref name="queryDefinition"/>.
		/// </summary>
		/// <typeparam name="T">The item type of model.</typeparam>
		/// <param name="containerName">Name of the container.</param>
		/// <param name="queryDefinition">The query definiton.</param>
		/// <param name="partitionId">The partition identifier.</param>
		/// <returns>The <see cref="IEnumerable{T}"/>.</returns>
		Task<IEnumerable<T>> GetItemsAsync<T>(string containerName, QueryDefinition queryDefinition, string partitionId = null);

		Task<IEnumerable<T>> GetAllItemsAsync<T>(string containerName);

        Task<IEnumerable<T>> GetAllItemsAsync<T>(string databaseName, string containerName);

		/// <summary>
		/// Gets the total records count based on the <paramref name="containerName"/>.
		/// </summary>
		/// <param name="containerName">Name of the container.</param>
		/// <returns>The count of records in the container.</returns>
		Task<int> GetCountAsync(string containerName);

        /// <summary>
        /// Gets the total records count based on the <paramref name="queryDefinition"/>.
        /// </summary>
        /// <param name="containerName">Name of the container.</param>
        /// <param name="queryDefinition">The query definition.</param>
        /// <returns>The count of records in the container.</returns>
        Task<int> GetCountByQueryAsync(string containerName, QueryDefinition queryDefinition = null);

        Task<bool> CheckIfDatabaseExists();

        Task<bool> CheckIfContainerExists(string containerName);

        Task<bool> IsContainerEmpty(string containerName);

		Task<bool> DeleteDatabaseIfExistsAsync();

        Task<bool> CreateDatabaseIfNotExistsAsync();

        Task<bool> DatabaseExistsAsync(string databaseName);

        Task<bool> BulkInsert(string containerName, string partitionKey, List<object> itemsToInsert, int chunkSize = 50);

		Task<bool> ContainerExistsAsync(string databaseName, string containerName);

		Task<bool> CreateStoredProcedureAsync(string containerName, string storedProcName, string storedProcContent);

        Task<bool> CreateContainerIfNotExistsAsync(string containerName, string partitionKey);

	}
}
