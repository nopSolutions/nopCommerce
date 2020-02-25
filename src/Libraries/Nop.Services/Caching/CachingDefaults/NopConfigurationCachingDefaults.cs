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
        public static string SettingsAllAsDictionaryCacheKey => "Nop.setting.all.";

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        public static string SettingsAllCacheKey => "Nop.setting.all";

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string SettingsPrefixCacheKey => "Nop.setting.";
    }
}