using System.ComponentModel.DataAnnotations;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Settings
{
    /// <summary>
    /// Represents a common configuration model
    /// </summary>
    public partial class CommonConfigModel : BaseNopModel, IConfigModel
    {
        #region Properties

        [NopResourceDisplayName("Admin.Configuration.AppSettings.Common.DisplayFullErrorStack")]
        public bool DisplayFullErrorStack { get; set; }

        [NopResourceDisplayName("Admin.Configuration.AppSettings.Common.AzureBlobStorageConnectionString")]
        public string AzureBlobStorageConnectionString { get; set; }

        [NopResourceDisplayName("Admin.Configuration.AppSettings.Common.AzureBlobStorageContainerName")]
        public string AzureBlobStorageContainerName { get; set; }

        [NopResourceDisplayName("Admin.Configuration.AppSettings.Common.AzureBlobStorageEndPoint")]
        public string AzureBlobStorageEndPoint { get; set; }

        [NopResourceDisplayName("Admin.Configuration.AppSettings.Common.AzureBlobStorageAppendContainerName")]
        public bool AzureBlobStorageAppendContainerName { get; set; }

        [NopResourceDisplayName("Admin.Configuration.AppSettings.Common.UseAzureBlobStorageToStoreDataProtectionKeys")]
        public bool UseAzureBlobStorageToStoreDataProtectionKeys { get; set; }

        [NopResourceDisplayName("Admin.Configuration.AppSettings.Common.AzureBlobStorageContainerNameForDataProtectionKeys")]
        public string AzureBlobStorageContainerNameForDataProtectionKeys { get; set; }

        [NopResourceDisplayName("Admin.Configuration.AppSettings.Common.AzureKeyVaultIdForDataProtectionKeys")]
        public string AzureKeyVaultIdForDataProtectionKeys { get; set; }

        [NopResourceDisplayName("Admin.Configuration.AppSettings.Common.RedisEnabled")]
        public bool RedisEnabled { get; set; }

        [NopResourceDisplayName("Admin.Configuration.AppSettings.Common.RedisConnectionString")]
        public string RedisConnectionString { get; set; }

        [NopResourceDisplayName("Admin.Configuration.AppSettings.Common.RedisDatabaseId")]
        [UIHint("Int32Nullable")]
        public int? RedisDatabaseId { get; set; }

        [NopResourceDisplayName("Admin.Configuration.AppSettings.Common.UseRedisToStoreDataProtectionKeys")]
        public bool UseRedisToStoreDataProtectionKeys { get; set; }

        [NopResourceDisplayName("Admin.Configuration.AppSettings.Common.UseRedisForCaching")]
        public bool UseRedisForCaching { get; set; }

        [NopResourceDisplayName("Admin.Configuration.AppSettings.Common.IgnoreRedisTimeoutException")]
        public bool IgnoreRedisTimeoutException { get; set; }

        [NopResourceDisplayName("Admin.Configuration.AppSettings.Common.UseRedisToStorePluginsInfo")]
        public bool UseRedisToStorePluginsInfo { get; set; }

        [NopResourceDisplayName("Admin.Configuration.AppSettings.Common.UserAgentStringsPath")]
        public string UserAgentStringsPath { get; set; }

        [NopResourceDisplayName("Admin.Configuration.AppSettings.Common.CrawlerOnlyUserAgentStringsPath")]
        public string CrawlerOnlyUserAgentStringsPath { get; set; }

        [NopResourceDisplayName("Admin.Configuration.AppSettings.Common.DisableSampleDataDuringInstallation")]
        public bool DisableSampleDataDuringInstallation { get; set; }

        [NopResourceDisplayName("Admin.Configuration.AppSettings.Common.PluginsIgnoredDuringInstallation")]
        public string PluginsIgnoredDuringInstallation { get; set; }

        [NopResourceDisplayName("Admin.Configuration.AppSettings.Common.ClearPluginShadowDirectoryOnStartup")]
        public bool ClearPluginShadowDirectoryOnStartup { get; set; }

        [NopResourceDisplayName("Admin.Configuration.AppSettings.Common.CopyLockedPluginAssembilesToSubdirectoriesOnStartup")]
        public bool CopyLockedPluginAssembilesToSubdirectoriesOnStartup { get; set; }

        [NopResourceDisplayName("Admin.Configuration.AppSettings.Common.UseUnsafeLoadAssembly")]
        public bool UseUnsafeLoadAssembly { get; set; }

        [NopResourceDisplayName("Admin.Configuration.AppSettings.Common.UsePluginsShadowCopy")]
        public bool UsePluginsShadowCopy { get; set; }

        [NopResourceDisplayName("Admin.Configuration.AppSettings.Common.UseSessionStateTempDataProvider")]
        public bool UseSessionStateTempDataProvider { get; set; }

        [NopResourceDisplayName("Admin.Configuration.AppSettings.Common.DefaultCacheTime")]
        public int DefaultCacheTime { get; set; }

        [NopResourceDisplayName("Admin.Configuration.AppSettings.Common.ShortTermCacheTime")]
        public int ShortTermCacheTime { get; set; }

        [NopResourceDisplayName("Admin.Configuration.AppSettings.Common.BundledFilesCacheTime")]
        public int BundledFilesCacheTime { get; set; }

        #endregion
    }
}