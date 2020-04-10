using Nop.Core.Caching;

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
        public static CacheKey ActivityTypeAllCacheKey => new CacheKey("Nop.activitytype.all");
    }
}