namespace Nop.Core.Configuration
{
    /// <summary>
    /// Represents Redis configuration parameters
    /// </summary>
    public partial class RedisConfig : IConfig
    {
        /// <summary>
        /// Gets or sets a value indicating whether we should use Redis server
        /// </summary>
        public bool Enabled { get; set; } = false;

        /// <summary>
        /// Gets or sets Redis connection string. Used when Redis is enabled
        /// </summary>
        public string ConnectionString { get; set; } = "127.0.0.1:6379,ssl=False";

        /// <summary>
        /// Gets or sets a specific redis database; If you need to use a specific redis database, just set its number here. set NULL if should use the different database for each data type (used by default)
        /// </summary>
        public int? DatabaseId { get; set; } = null;

        /// <summary>
        /// Gets or sets a value indicating whether we should use Redis server for caching (instead of default in-memory caching)
        /// </summary>
        public bool UseCaching { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether the data protection system should be configured to persist keys in the Redis database
        /// </summary>
        public bool StoreDataProtectionKeys { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether we should use Redis server for store the plugins info (instead of default plugin.json file)
        /// </summary>
        public bool StorePluginsInfo { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether we should ignore Redis timeout exception (Enabling this setting increases cache stability but may decrease site performance)
        /// </summary>
        public bool IgnoreTimeoutException { get; set; } = false;
    }
}