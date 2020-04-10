using Nop.Core.Caching;

namespace Nop.Services.Caching.CachingDefaults
{
    /// <summary>
    /// Represents default values related to Gdpr services
    /// </summary>
    public static partial class NopGdprCachingDefaults
    {
        /// <summary>
        /// Gets a key for caching
        /// </summary>
        public static CacheKey ConsentsAllCacheKey => new CacheKey("Nop.consents.all");
    }
}