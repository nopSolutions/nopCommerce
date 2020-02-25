namespace Nop.Services.Caching.CachingDefaults
{
    /// <summary>
    /// Represents default values related to logging services
    /// </summary>
    public static partial class NopLoggingCachingDefaults
    {
        /// <summary>
        /// Gets a key for caching
        /// </summary>
        public static string ActivityTypeAllCacheKey => "Nop.activitytype.all";

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string ActivityTypePrefixCacheKey => "Nop.activitytype.";
    }
}