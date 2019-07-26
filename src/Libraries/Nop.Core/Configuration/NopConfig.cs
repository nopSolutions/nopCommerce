using Newtonsoft.Json;

namespace Nop.Core.Configuration
{
    /// <summary>
    /// Represents startup Nop configuration parameters
    /// </summary>
    public partial class NopConfig
    {
        /// <summary>
        /// Gets or sets a value indicating whether to display the full error in production environment.
        /// It's ignored (always enabled) in development environment
        /// </summary>
        public bool DisplayFullErrorStack { get; set; }
        
        /// <summary>
        /// Gets or sets connection string for Azure BLOB storage
        /// </summary>
        public string AzureBlobStorageConnectionString { get; set; }
        /// <summary>
        /// Gets or sets container name for Azure BLOB storage
        /// </summary>
        public string AzureBlobStorageContainerName { get; set; }
        /// <summary>
        /// Gets or sets end point for Azure BLOB storage
        /// </summary>
        public string AzureBlobStorageEndPoint { get; set; }
        /// <summary>
        /// Gets or sets whether or the Container Name is appended to the AzureBlobStorageEndPoint
        /// when constructing the url
        /// </summary>
        public bool AzureBlobStorageAppendContainerName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether we should use Redis server
        /// </summary>
        public bool RedisEnabled { get; set; }
        /// <summary>
        /// Gets or sets Redis connection string. Used when Redis is enabled
        /// </summary>
        public string RedisConnectionString { get; set; }
        /// <summary>
        /// Gets or sets a specific redis database; If you need to use a specific redis database, just set its number here. set NULL if should use the different database for each data type (used by default)
        /// </summary>
        public int? RedisDatabaseId { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether the data protection system should be configured to persist keys in the Redis database
        /// </summary>
        public bool UseRedisToStoreDataProtectionKeys { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether we should use Redis server for caching (instead of default in-memory caching)
        /// </summary>
        public bool UseRedisForCaching { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether we should use Redis server for store the plugins info (instead of default plugin.json file)
        /// </summary>
        public bool UseRedisToStorePluginsInfo { get; set; }

        /// <summary>
        /// Gets or sets path to database with user agent strings
        /// </summary>
        public string UserAgentStringsPath { get; set; }
        /// <summary>
        /// Gets or sets path to database with crawler only user agent strings
        /// </summary>
        public string CrawlerOnlyUserAgentStringsPath { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether a store owner can install sample data during installation
        /// </summary>
        public bool DisableSampleDataDuringInstallation { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether to use fast installation. 
        /// By default this setting should always be set to "False" (only for advanced users)
        /// </summary>
        public bool UseFastInstallationService { get; set; }
        /// <summary>
        /// Gets or sets a list of plugins ignored during nopCommerce installation
        /// </summary>
        public string PluginsIgnoredDuringInstallation { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to clear /Plugins/bin directory on application startup
        /// </summary>
        public bool ClearPluginShadowDirectoryOnStartup { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to copy "locked" assemblies from /Plugins/bin directory to temporary subdirectories on application startup
        /// </summary>
        public bool CopyLockedPluginAssembilesToSubdirectoriesOnStartup { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to load an assembly into the load-from context, bypassing some security checks.
        /// </summary>
        public bool UseUnsafeLoadAssembly { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to copy plugins library to the /Plugins/bin directory on application startup
        /// </summary>
        public bool UsePluginsShadowCopy { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to use backwards compatibility with SQL Server 2008 and SQL Server 2008R2
        /// </summary>
        public bool UseRowNumberForPaging { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to store TempData in the session state.
        /// By default the cookie-based TempData provider is used to store TempData in cookies.
        /// </summary>
        public bool UseSessionStateTempDataProvider { get; set; }
        
        /// <summary>
        /// Gets a value indicating whether we should use Azure blob storage
        /// </summary>
        [JsonIgnore]
        public bool AzureBlobStorageEnabled => !string.IsNullOrEmpty(AzureBlobStorageConnectionString);
    }
}