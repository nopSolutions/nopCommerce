using Newtonsoft.Json;

namespace Nop.Core.Configuration
{
    /// <summary>
    /// Represents common configuration parameters
    /// </summary>
    public partial class NopConfig : IConfig
    {
        /// <summary>
        /// Gets or sets a value indicating whether to display the full error in production environment.
        /// It's ignored (always enabled) in development environment
        /// </summary>
        public bool DisplayFullErrorStack { get; set; } = false;

        /// <summary>
        /// Gets or sets connection string for Azure BLOB storage
        /// </summary>
        public string AzureBlobStorageConnectionString { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets container name for Azure BLOB storage
        /// </summary>
        public string AzureBlobStorageContainerName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets end point for Azure BLOB storage
        /// </summary>
        public string AzureBlobStorageEndPoint { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets whether or the Container Name is appended to the AzureBlobStorageEndPoint
        /// when constructing the url
        /// </summary>
        public bool AzureBlobStorageAppendContainerName { get; set; } = true;

        /// <summary>
        /// Gets or sets whether to store Data Protection Keys in Azure Blob Storage (the UseRedisToStoreDataProtectionKeys option should be disabled)
        /// </summary>
        public bool UseAzureBlobStorageToStoreDataProtectionKeys { get; set; } = false;

        /// <summary>
        /// Gets or sets the Azure container name for storing Data Prtection Keys (this container should be separate from the container used for media and should be Private)
        /// </summary>
        public string AzureBlobStorageContainerNameForDataProtectionKeys { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the Azure key vault ID used to encrypt the Data Protection Keys. (this is optional)
        /// </summary>
        public string AzureKeyVaultIdForDataProtectionKeys { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether we should use Redis server
        /// </summary>
        public bool RedisEnabled { get; set; } = false;

        /// <summary>
        /// Gets or sets Redis connection string. Used when Redis is enabled
        /// </summary>
        public string RedisConnectionString { get; set; } = "127.0.0.1:6379,ssl=False";

        /// <summary>
        /// Gets or sets a specific redis database; If you need to use a specific redis database, just set its number here. set NULL if should use the different database for each data type (used by default)
        /// </summary>
        public int? RedisDatabaseId { get; set; } = null;

        /// <summary>
        /// Gets or sets a value indicating whether the data protection system should be configured to persist keys in the Redis database
        /// </summary>
        public bool UseRedisToStoreDataProtectionKeys { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether we should use Redis server for caching (instead of default in-memory caching)
        /// </summary>
        public bool UseRedisForCaching { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether we should ignore Redis timeout exception (Enabling this setting increases cache stability but may decrease site performance)
        /// </summary>
        public bool IgnoreRedisTimeoutException { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether we should use Redis server for store the plugins info (instead of default plugin.json file)
        /// </summary>
        public bool UseRedisToStorePluginsInfo { get; set; } = false;

        /// <summary>
        /// Gets or sets path to database with user agent strings
        /// </summary>
        public string UserAgentStringsPath { get; set; } = "~/App_Data/browscap.xml";

        /// <summary>
        /// Gets or sets path to database with crawler only user agent strings
        /// </summary>
        public string CrawlerOnlyUserAgentStringsPath { get; set; } = "~/App_Data/browscap.crawlersonly.xml";

        /// <summary>
        /// Gets or sets a value indicating whether a store owner can install sample data during installation
        /// </summary>
        public bool DisableSampleDataDuringInstallation { get; set; } = false;

        /// <summary>
        /// Gets or sets a list of plugins ignored during nopCommerce installation
        /// </summary>
        public string PluginsIgnoredDuringInstallation { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether to clear /Plugins/bin directory on application startup
        /// </summary>
        public bool ClearPluginShadowDirectoryOnStartup { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether to copy "locked" assemblies from /Plugins/bin directory to temporary subdirectories on application startup
        /// </summary>
        public bool CopyLockedPluginAssembilesToSubdirectoriesOnStartup { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether to load an assembly into the load-from context, bypassing some security checks.
        /// </summary>
        public bool UseUnsafeLoadAssembly { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether to copy plugins library to the /Plugins/bin directory on application startup
        /// </summary>
        public bool UsePluginsShadowCopy { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether to store TempData in the session state.
        /// By default the cookie-based TempData provider is used to store TempData in cookies.
        /// </summary>
        public bool UseSessionStateTempDataProvider { get; set; } = false;

        /// <summary>
        /// Gets or sets a value that indicates whether to use MiniProfiler services
        /// </summary>
        public bool MiniProfilerEnabled { get; set; } = false;

        /// <summary>
        /// Gets a value indicating whether we should use Azure blob storage
        /// </summary>
        [JsonIgnore]
        public bool AzureBlobStorageEnabled => !string.IsNullOrEmpty(AzureBlobStorageConnectionString);

        /// <summary>
        /// Whether to use an Azure Key Vault to encrypt the Data Protection Keys
        /// </summary>
        [JsonIgnore]
        public bool EncryptDataProtectionKeysWithAzureKeyVault => !string.IsNullOrEmpty(AzureKeyVaultIdForDataProtectionKeys);

        /// <summary>
        /// Gets or sets the default cache time in minutes
        /// </summary>
        public int DefaultCacheTime { get; set; } = 60;

        /// <summary>
        /// Gets or sets the short term cache time in minutes
        /// </summary>
        public int ShortTermCacheTime { get; set; } = 3;

        /// <summary>
        /// Gets or sets the bundled files cache time in minutes
        /// </summary>
        public int BundledFilesCacheTime { get; set; } = 120;
    }
}