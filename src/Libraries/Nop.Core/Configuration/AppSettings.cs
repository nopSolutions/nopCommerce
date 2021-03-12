using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Nop.Core.Configuration
{
    /// <summary>
    /// Represents the app settings
    /// </summary>
    public partial class AppSettings
    {
        #region Properties

        /// <summary>
        /// Gets or sets cache configuration parameters
        /// </summary>
        public CacheConfig CacheConfig { get; set; } = new CacheConfig();

        /// <summary>
        /// Gets or sets hosting configuration parameters
        /// </summary>
        public HostingConfig HostingConfig { get; set; } = new HostingConfig();

        /// <summary>
        /// Gets or sets distributed cache configuration parameters
        /// </summary>
        public DistributedCacheConfig DistributedCacheConfig { get; set; } = new DistributedCacheConfig();

        /// <summary>
        /// Gets or sets Azure Blob storage configuration parameters
        /// </summary>
        public AzureBlobConfig AzureBlobConfig { get; set; } = new AzureBlobConfig();

        /// <summary>
        /// Gets or sets installation configuration parameters
        /// </summary>
        public InstallationConfig InstallationConfig { get; set; } = new InstallationConfig();

        /// <summary>
        /// Gets or sets plugin configuration parameters
        /// </summary>
        public PluginConfig PluginConfig { get; set; } = new PluginConfig();

        /// <summary>
        /// Gets or sets common configuration parameters
        /// </summary>
        public CommonConfig CommonConfig { get; set; } = new CommonConfig();

        /// <summary>
        /// Gets or sets additional configuration parameters
        /// </summary>
        [JsonExtensionData]
        public IDictionary<string, JToken> AdditionalData { get; set; }

        #endregion
    }
}