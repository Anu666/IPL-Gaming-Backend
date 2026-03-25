namespace IPL.Gaming.Common.Models.Repository
{
    public class CosmosDbSettings
    {
        /// <summary>
        /// Gets or sets the cosmos database end point URL.
        /// </summary>
        public string CosmosDbEndPointUrl { get; set; }

        /// <summary>
        /// Gets or sets the cosmos database authorization key.
        /// </summary>
        public string CosmosDbAuthorizationKey { get; set; }

        /// <summary>
        /// Gets or sets the cosmos database name.
        /// </summary>
        public string CosmosDbName { get; set; }

        /// <summary>
        /// Gets or sets the database throughput.
        /// </summary>
        public int DatabaseThroughput { get; set; }
    }
}
