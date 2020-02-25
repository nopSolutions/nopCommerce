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
        /// <remarks>
        /// {0} : entity ID
        /// {1} : entity name
        /// {2} : language ID
        /// </remarks>
        public static string UrlRecordActiveByIdNameLanguageCacheKey => "Nop.urlrecord.active.id-name-language-{0}-{1}-{2}";

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        public static string UrlRecordAllCacheKey => "Nop.urlrecord.all";

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : IDs hash
        /// </remarks>
        public static string UrlRecordByIdsCacheKey => "Nop.urlrecord.byids-{0}";

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : slug
        /// </remarks>
        public static string UrlRecordBySlugCacheKey => "Nop.urlrecord.active.slug-{0}";

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string UrlRecordPrefixCacheKey => "Nop.urlrecord.";

        #endregion
    }
}