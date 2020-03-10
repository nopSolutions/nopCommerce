using Nop.Core.Caching;

namespace Nop.Services.Caching.CachingDefaults
{
    /// <summary>
    /// Represents default values related to configuration services
    /// </summary>
    public static partial class NopConfigurationCachingDefaults
    {
        /// <summary>
        /// Gets a key for caching
        /// </summary>
        public static CacheKey SettingsAllAsDictionaryCacheKey => new CacheKey("Nop.setting.all.as.dictionary", SettingsPrefixCacheKey);

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        public static CacheKey SettingsAllCacheKey => new CacheKey("Nop.setting.all", SettingsPrefixCacheKey);

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string SettingsPrefixCacheKey => "Nop.setting.";
    }
}