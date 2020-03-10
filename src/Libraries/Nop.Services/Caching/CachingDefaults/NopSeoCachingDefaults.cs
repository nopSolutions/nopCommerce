using Nop.Core.Caching;

namespace Nop.Services.Caching.CachingDefaults
{
    /// <summary>
    /// Represents default values related to SEO services
    /// </summary>
    public static partial class NopSeoCachingDefaults
    {
        #region URL records
        
        /// <summary>
        /// Gets a key for caching
        /// </summary>
        public static CacheKey UrlRecordAllCacheKey => new CacheKey("Nop.urlrecord.all");

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : entity ID
        /// {1} : entity name
        /// {2} : language ID
        /// </remarks>
        public static CacheKey UrlRecordActiveByIdNameLanguageCacheKey => new CacheKey("Nop.urlrecord.active.id-name-language-{0}-{1}-{2}");

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : IDs hash
        /// </remarks>
        public static CacheKey UrlRecordByIdsCacheKey => new CacheKey("Nop.urlrecord.byids-{0}", UrlRecordByIdsPrefixCacheKey);

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string UrlRecordByIdsPrefixCacheKey => "Nop.urlrecord.byids";

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : slug
        /// </remarks>
        public static CacheKey UrlRecordBySlugCacheKey => new CacheKey("Nop.urlrecord.active.slug-{0}");

        #endregion
    }
}