using IPL.Gaming.Common.Models.CosmosDB;
using IPL.Gaming.Common.Models.Repository;
using IPL.Gaming.Database.Interfaces;
using IPL.Gaming.Database.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.Azure.Cosmos.Scripts;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace IPL.Gaming.Database
{
    public class CosmosService : ICosmosService
    {
        #region Private Properties

        /// <summary>
        /// The cosmos database settings.
        /// </summary>
        private CosmosDbSettings cosmosDBSettings;

        /// <summary>
        /// The cosmos client.
        /// </summary>
        private static CosmosClient cosmosClient;

        /// <summary>
        /// The database.
        /// </summary>
        private Microsoft.Azure.Cosmos.Database cosmosDatabase;

        /// <summary>
        /// The container.
        /// </summary>
        private Container container;

        /// <summary>
        /// The status of dispose.
        /// </summary>
        private bool disposed;

        public IConfiguration _config { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="CosmosDbService"/> class.
        /// </summary>
        /// <param name="cosmosDBSettings">The cosmos database settings.</param>
        public CosmosService(IConfiguration config)
        {
            _config = config;
            this.cosmosDBSettings = new CosmosDbSettings()
            {
                CosmosDbEndPointUrl = _config["CosmosDbSettings:ENDPOINT_URL"],
                CosmosDbAuthorizationKey = _config["CosmosDbSettings:AUTHORIZATION_KEY"],
                CosmosDbName = _config["CosmosDbSettings:DATABASE_NAME"],
                DatabaseThroughput = Convert.ToInt32(_config["CosmosDbSettings:DATABASE_THROUGHPUT"]),
            };

            if (cosmosClient == null)
            {
                var clientBuilder = new CosmosClientBuilder(this.cosmosDBSettings.CosmosDbEndPointUrl, this.cosmosDBSettings.CosmosDbAuthorizationKey);
                cosmosClient = clientBuilder
                                    .WithConnectionModeDirect()
                                    .WithBulkExecution(true)
                                    .WithConsistencyLevel(ConsistencyLevel.Session)
                                    .Build();
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Adds the <paramref name="item"/> asynchronously.
        /// </summary>
        /// <typeparam name="T">The item type of model.</typeparam>
        /// <param name="containerName">Name of the container.</param>
        /// <param name="item">The item.</param>
        /// <param name="partitionId">The partition identifier.</param>
        /// <returns>The added item.</returns>
        public async Task<T> AddItemAsync<T>(string containerName, T item, string partitionId = null)
        {
            this.SetContainerReference(containerName);
            var partitionKey = string.IsNullOrWhiteSpace(partitionId) ? (PartitionKey?)null : new PartitionKey(partitionId);

            var response = await this.container.CreateItemAsync<T>(item, partitionKey);
            return response.Resource;
        }

        public async Task<T> AddItemAsync<T>(string containerName, T item, int partitionId)
        {
            this.SetContainerReference(containerName);
            var partitionKey = new PartitionKey(partitionId);
            var response = await this.container.CreateItemAsync<T>(item, partitionKey);
            return response.Resource;
        }

        /// <summary>
        /// Upserts the <paramref name="item"/> asynchronously.
        /// </summary>
        /// <typeparam name="T">The item type of model.</typeparam>
        /// <param name="containerName">Name of the container.</param>
        /// <param name="item">The item.</param>
        /// <param name="partitionId">The partition identifier.</param>
        /// <returns>The updated item.</returns>
        public async Task<T> UpsertItemAsync<T>(string containerName, T item, string partitionId = null)
        {
            this.SetContainerReference(containerName);
            var partitionKey = string.IsNullOrWhiteSpace(partitionId) ? (PartitionKey?)null : new PartitionKey(partitionId);

            var response = await this.container.UpsertItemAsync<T>(item, partitionKey);
            return response.Resource;
        }

        /// <summary>
        /// Upserts the <paramref name="item"/> asynchronously.
        /// </summary>
        /// <typeparam name="T">The item type of model.</typeparam>
        /// <param name="containerName">Name of the container.</param>
        /// <param name="item">The item.</param>
        /// <param name="partitionId">The partition identifier.</param>
        /// <returns>The updated item.</returns>
        public async Task<T> UpsertItemAsync<T>(string containerName, T item, int partitionId)
        {
            this.SetContainerReference(containerName);
            var partitionKey = new PartitionKey(partitionId);
            var response = await this.container.UpsertItemAsync<T>(item, partitionKey);
            return response.Resource;
        }

        /// <summary>
        /// Deletes the item based on the <paramref name="id"/> asynchronously.
        /// </summary>
        /// <typeparam name="T">The item type of model.</typeparam>
        /// <param name="containerName">Name of the container.</param>
        /// <param name="id">The identifier.</param>
        /// <param name="partitionId">The partition identifier.</param>
        /// <returns>The deleted item.</returns>
        public async Task<T> DeleteItemAsync<T>(string containerName, string id, string partitionId)
        {
            this.SetContainerReference(containerName);

            var response = await this.container.DeleteItemAsync<T>(id, new PartitionKey(partitionId));
            return response.Resource;
        }

        /// <summary>
        /// Deletes the items based on the <paramref name="containerName"/>, <paramref name="query"/> and <paramref name="partitionId"/> asynchronously.
        /// </summary>
        /// <typeparam name="T">The item type of model.</typeparam>
        /// <param name="containerName">Name of the container.</param>
        /// <param name="query">The query.</param>
        /// <param name="partitionId">The partition identifier.</param>
        /// <returns>Success or not.</returns>
        public async Task<bool> DeleteItemsByQueryAsync(string containerName, string query, string partitionId)
        {
            this.SetContainerReference(containerName);
            var isSuccess = true;

            try
            {
                bool resume = true;
                do
                {
                    StoredProcedureExecuteResponse<DeleteStatus> result = await this.container.Scripts.ExecuteStoredProcedureAsync<DeleteStatus>("bulkDelete", new PartitionKey(partitionId), new dynamic[] { query });
                    resume = result.Resource.Continuation;
                }
                while (resume);
            }
            catch (Exception ex)
            {
                isSuccess = false;
            }

            return isSuccess;
        }

        /// <summary>
        /// Deletes the items based on the <paramref name="containerName"/>, <paramref name="query"/> and <paramref name="partitionId"/> asynchronously.
        /// </summary>
        /// <typeparam name="T">The item type of model.</typeparam>
        /// <param name="containerName">Name of the container.</param>
        /// <param name="query">The query.</param>
        /// <param name="partitionId">The partition identifier.</param>
        /// <returns>Success or not.</returns>
        public async Task<bool> DeleteItemsByQueryAsync(string containerName, string query, int partitionId)
        {
            this.SetContainerReference(containerName);
            var isSuccess = true;

            try
            {
                bool resume = true;
                do
                {
                    StoredProcedureExecuteResponse<DeleteStatus> result = await this.container.Scripts.ExecuteStoredProcedureAsync<DeleteStatus>("bulkDelete", new PartitionKey(partitionId), new dynamic[] { query });
                    resume = result.Resource.Continuation;
                }
                while (resume);
            }
            catch (Exception ex)
            {
                isSuccess = false;
            }

            return isSuccess;
        }

        /// <summary>
        /// Gets the item based on the <paramref name="id"/> asynchronously.
        /// </summary>
        /// <typeparam name="T">The item type of model.</typeparam>
        /// <param name="containerName">Name of the container.</param>
        /// <param name="id">The identifier.</param>
        /// <param name="partitionId">The partition identifier.</param>
        /// <returns>The item.</returns>
        public async Task<T> GetItemAsync<T>(string containerName, string id, string partitionId)
        {
            try
            {
                this.SetContainerReference(containerName);

                var response = await this.container.ReadItemAsync<T>(id, new PartitionKey(partitionId));
                return response.Resource;
            }
            catch (CosmosException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return default;
                }
                else
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Gets the item based on the <paramref name="id"/> asynchronously.
        /// </summary>
        /// <typeparam name="T">The item type of model.</typeparam>
        /// <param name="containerName">Name of the container.</param>
        /// <param name="id">The identifier.</param>
        /// <param name="partitionId">The partition identifier.</param>
        /// <returns>The item.</returns>
        public async Task<T> GetItemByIdAsync<T>(string containerName, string id, string idPropertyName = "id")
        {
            try
            {
                this.SetContainerReference(containerName);

                var queryString = $"SELECT * FROM O where O.{idPropertyName} = @id";
                var queryDefinition = new QueryDefinition(queryString)
                                            .WithParameter("@id", id);
                var items = await this.GetItemsAsync<T>(containerName, queryDefinition);
                return items.FirstOrDefault();
            }
            catch (CosmosException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return default;
                }
                else
                {
                    throw;
                }
            }
        }

        public async Task<IEnumerable<T>> GetItemsAsync<T>(string databaseName, string containerName, QueryDefinition queryDefinition, string partitionId = null)
        {
            var containerInstance = cosmosClient.GetContainer(databaseName, containerName);
            var partitionKey = string.IsNullOrWhiteSpace(partitionId) ? (PartitionKey?)null : new PartitionKey(partitionId);

            var queryResult = containerInstance.GetItemQueryIterator<T>(
                queryDefinition,
                null,
                new QueryRequestOptions
                {
                    // https://docs.microsoft.com/en-us/dotnet/api/microsoft.azure.cosmos.queryrequestoptions?view=azure-dotnet
                    // https://feedback.azure.com/forums/263030-azure-cosmos-db/suggestions/38633590-allow-multiple-partitionkey-objects-in-queryreques
                    // https://docs.microsoft.com/en-us/azure/cosmos-db/sql-api-query-metrics

                    MaxConcurrency = -1,
                    MaxItemCount = -1,
                    PartitionKey = partitionKey,
                });

            var results = new List<T>();
            while (queryResult.HasMoreResults)
            {
                var response = await queryResult.ReadNextAsync();
                results.AddRange(response.ToList());
            }

            return results;
        }

        /// <summary>
        /// Gets the items asynchronously based on the <paramref name="queryDefinition"/>.
        /// </summary>
        /// <typeparam name="T">The item type of model.</typeparam>
        /// <param name="containerName">Name of the container.</param>
        /// <param name="queryDefinition">The query definiton.</param>
        /// <param name="partitionId">The partition identifier.</param>
        /// <returns>The <see cref="IEnumerable{T}"/>.</returns>
        public async Task<IEnumerable<T>> GetItemsAsync<T>(string containerName, QueryDefinition queryDefinition, string partitionId = null)
        {
            this.SetContainerReference(containerName);
            var partitionKey = string.IsNullOrWhiteSpace(partitionId) ? (PartitionKey?)null : new PartitionKey(partitionId);

            var queryResult = this.container.GetItemQueryIterator<T>(
                queryDefinition,
                null,
                new QueryRequestOptions
                {
                    // https://docs.microsoft.com/en-us/dotnet/api/microsoft.azure.cosmos.queryrequestoptions?view=azure-dotnet
                    // https://feedback.azure.com/forums/263030-azure-cosmos-db/suggestions/38633590-allow-multiple-partitionkey-objects-in-queryreques
                    // https://docs.microsoft.com/en-us/azure/cosmos-db/sql-api-query-metrics

                    MaxConcurrency = -1,
                    MaxItemCount = -1,
                    PartitionKey = partitionKey,
                });

            var results = new List<T>();
            while (queryResult.HasMoreResults)
            {
                var response = await queryResult.ReadNextAsync();
                results.AddRange(response.ToList());
            }

            return results;
        }

        public async Task<IEnumerable<T>> GetAllItemsAsync<T>(string containerName)
        {
            var getAllQueryDefinition = new QueryDefinition($"select * from X");
            var items = await this.GetItemsAsync<T>(containerName, getAllQueryDefinition);
            return items;
        }

        public async Task<IEnumerable<T>> GetAllItemsAsync<T>(string databaseName, string containerName)
        {
            var getAllQueryDefinition = new QueryDefinition($"select * from X");
            var items = await this.GetItemsAsync<T>(databaseName, containerName, getAllQueryDefinition);
            return items;
        }

        /// <summary>
        /// Gets the total records count based on the <paramref name="containerName"/>.
        /// </summary>
        /// <param name="containerName">Name of the container.</param>
        /// <returns>The count of records in the container.</returns>
        public async Task<int> GetCountAsync(string containerName)
        {
            this.SetContainerReference(containerName);

            var queryDefinition = new QueryDefinition($"select value count(1) from X");

            var queryResult = this.container.GetItemQueryIterator<int>(
                queryDefinition,
                null,
                new QueryRequestOptions
                {
                    MaxConcurrency = -1,
                    MaxItemCount = -1,
                });

            if (queryResult.HasMoreResults)
            {
                var response = await queryResult.ReadNextAsync();
                return response.Resource.First();
            }

            // As HasMoreResults property is false 
            return 0;
        }

        /// <summary>
        /// Gets the total records count based on the <paramref name="queryDefinition"/>.
        /// </summary>
        /// <param name="containerName">Name of the container.</param>
        /// <param name="queryDefinition">The query definition.</param>
        /// <returns>The count of records in the container.</returns>
        public async Task<int> GetCountByQueryAsync(string containerName, QueryDefinition queryDefinition = null)
        {
            queryDefinition = queryDefinition ?? new QueryDefinition($"select value count(1) from X");
            this.SetContainerReference(containerName);

            var queryResult = this.container.GetItemQueryIterator<int>(
                queryDefinition,
                null,
                new QueryRequestOptions
                {
                    MaxConcurrency = -1,
                    MaxItemCount = -1,
                });

            if (queryResult.HasMoreResults)
            {
                var response = await queryResult.ReadNextAsync();
                return response.Resource.FirstOrDefault();
            }

            // As HasMoreResults property is false 
            return 0;
        }

        public async Task<bool> IsContainerEmpty(string containerName)
        {
            var count = await this.GetCountAsync(containerName);
            return count == 0;
        }

        public async Task<bool> DeleteContainerAsync(ContainerDetail container)
        {
            return await this.DeleteContainersAsync(new List<ContainerDetail>() { container });
        }

        public async Task<bool> DeleteContainersAsync(List<ContainerDetail> containers)
        {
            this.SetDatabaseReference();
            var isSuccess = true;

            foreach (var containerRecord in containers)
            {
                try
                {
                    var containerInstance = cosmosClient.GetContainer(this.cosmosDBSettings.CosmosDbName, containerRecord.Name);
                    await containerInstance.DeleteContainerAsync();
                }
                catch (Exception ex)
                {
                    // TODO: Identify other types of errors instead of notfound
                    // ignore and delete other collections
                    isSuccess = false;
                }
            }

            return isSuccess;
        }

        public async Task<bool> CheckIfDatabaseExists()
        {
            return await DatabaseExistsAsync(this.cosmosDBSettings.CosmosDbName);
        }

        public async Task<bool> CheckIfContainerExists(string containerName)
        {
            return await ContainerExistsAsync(this.cosmosDBSettings.CosmosDbName, containerName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Is database deleted.. returns true even if database didn't exist</returns>
        public async Task<bool> DeleteDatabaseIfExistsAsync()
        {
            try
            {
                if (!(await CheckIfDatabaseExists()))
                {
                    return true;
                }

                this.SetDatabaseReference();
                DatabaseResponse response = await this.cosmosDatabase.DeleteAsync();
                return response != null;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Returns false if database already exists</returns>
		public async Task<bool> CreateDatabaseIfNotExistsAsync()
        {
            try
            {
                if (await CheckIfDatabaseExists())
                {
                    return false;
                }

                var databaseResponse = await cosmosClient.CreateDatabaseIfNotExistsAsync(this.cosmosDBSettings.CosmosDbName, this.cosmosDBSettings.DatabaseThroughput);
                return databaseResponse != null;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> CreateStoredProcedureAsync(string containerName, string storedProcName, string storedProcContent)
        {
            this.SetContainerReference(containerName);
            var isSuccess = true;

            try
            {
                StoredProcedureProperties storedProcedure = new StoredProcedureProperties(storedProcName, storedProcContent);
                StoredProcedureResponse storedProcedureResponse = await this.container.Scripts.CreateStoredProcedureAsync(storedProcedure);
            }
            catch (Exception ex)
            {
                isSuccess = false;
            }

            return isSuccess;
        }

        public async Task<bool> CreateContainerIfNotExistsAsync(string containerName, string partitionKey)
        {
            this.SetContainerReference(containerName);
            var isSuccess = true;

            try
            {
                await this.cosmosDatabase.CreateContainerIfNotExistsAsync(containerName, $"/{partitionKey}");
            }
            catch (Exception ex)
            {
                isSuccess = false;
            }

            return isSuccess;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Sets the container reference.
        /// </summary>
        /// <param name="containerName">Name of the container.</param>
        private void SetContainerReference(string containerName)
        {
            if (this.cosmosDatabase == null)
            {
                this.cosmosDatabase = cosmosClient.GetDatabase(this.cosmosDBSettings.CosmosDbName);
            }

            this.container = cosmosClient.GetContainer(this.cosmosDBSettings.CosmosDbName, containerName);
        }

        /// <summary>
        /// Sets the database reference.
        /// </summary>
        private void SetDatabaseReference()
        {
            if (this.cosmosDatabase == null)
            {
                this.cosmosDatabase = cosmosClient.GetDatabase(this.cosmosDBSettings.CosmosDbName);
            }
        }

        private void SetDatabaseReference(string databaseName)
        {
            if (this.cosmosDatabase == null)
            {
                this.cosmosDatabase = cosmosClient.GetDatabase(databaseName);
            }
        }

        /// <summary>
        /// Checks if a database exists.
        /// </summary>
        /// <param name="databaseName">Name of the database to check.</param>
        /// <returns>True, if the database exists, otherwise false.</returns>
        public async Task<bool> DatabaseExistsAsync(string databaseName)
        {
            this.SetDatabaseReference();
            var databaseNames = new List<string>();
            using (FeedIterator<DatabaseProperties> iterator = cosmosClient.GetDatabaseQueryIterator<DatabaseProperties>())
            {
                while (iterator.HasMoreResults)
                {
                    foreach (DatabaseProperties databaseProperties in await iterator.ReadNextAsync())
                    {
                        databaseNames.Add(databaseProperties.Id);
                    }
                }
            }

            return databaseNames.Contains(databaseName);
        }

        /// <summary>
        /// Checks if a container exists.
        /// </summary>
        /// <param name="containerName">Name of the container to check.</param>
        /// <returns>True, if the container exists, otherwise false.</returns>
        public async Task<bool> ContainerExistsAsync(string databaseName, string containerName)
        {
            this.SetContainerReference(containerName);
            var databaseExists = await this.DatabaseExistsAsync(databaseName);
            if (!databaseExists)
            {
                return false;
            }

            var containerNames = new List<string>();
            var database = cosmosClient.GetDatabase(databaseName);
            using (FeedIterator<ContainerProperties> iterator = database.GetContainerQueryIterator<ContainerProperties>())
            {
                while (iterator.HasMoreResults)
                {
                    foreach (ContainerProperties containerProperties in await iterator.ReadNextAsync())
                    {
                        containerNames.Add(containerProperties.Id);
                    }
                }
            }

            return containerNames.Contains(containerName);
        }

        public async Task<bool> BulkInsert(string containerName, string partitionKey, List<object> itemsToInsert, int chunkSize = 50)
        {
            try
            {
                this.SetDatabaseReference();
                Container container = this.cosmosDatabase.GetContainer(containerName);
                List<Task> tasks = new List<Task>(itemsToInsert.Count);

                foreach (var chunk in itemsToInsert.Chunk(chunkSize)) //Returns a chunk with the correct size. 
                {
                    try
                    {
                        foreach (var item in chunk)
                        {
                            tasks.Add(container.CreateItemAsync(item)
                                .ContinueWith(itemResponse =>
                                {
                                    if (!itemResponse.IsCompletedSuccessfully)
                                    {
                                        //_logger.LogError($"Failed to create item - {JsonConvert.SerializeObject(item)}");

                                        AggregateException innerExceptions = itemResponse.Exception.Flatten();
                                        if (innerExceptions.InnerExceptions.FirstOrDefault(innerEx => innerEx is CosmosException) is CosmosException cosmosException)
                                        {
                                            //_logger.LogError($"Received {cosmosException.StatusCode} ({cosmosException.Message}).");
                                        }
                                        else
                                        {
                                            //_logger.LogError($"Exception {innerExceptions.InnerExceptions.FirstOrDefault()}.");
                                        }
                                    }
                                }));
                        }

                        // Wait until all are done
                        await Task.WhenAll(tasks);
                    }
                    catch (Exception ex)
                    {
                        //_logger.LogError(ex.Message);
                        continue;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex);
                return false;
            }
        }

        #endregion

        #region Disposable Methods

#pragma warning disable SA1202// Elements should be ordered by access.
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            // If this function is being called the user wants to release the
            // resources. lets call the Dispose which will do this for us.
            this.Dispose(true);

            // Now since we have done the cleanup already there is nothing left
            // for the Finalizer to do. So lets tell the GC not to call it later.
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if ((!this.disposed) && disposing)
            {
                if (cosmosClient != null)
                {
                    cosmosClient.Dispose();
                }
            }

            // dispose unmanaged resources
            this.disposed = true;
        }

        #endregion
    }
}

