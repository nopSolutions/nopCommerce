using Nop.Core.Caching;

namespace Nop.Services.News
{
    /// <summary>
    /// Represents default values related to orders services
    /// </summary>
    public static partial class NopNewsDefaults
    {
        #region Caching defaults

        /// <summary>
        /// Key for number of news comments
        /// </summary>
        /// <remarks>
        /// {0} : news item ID
        /// {1} : store ID
        /// {2} : are only approved comments?
        /// </remarks>
        public static CacheKey NewsCommentsNumberCacheKey => new CacheKey("Nop.news.comments.number-{0}-{1}-{2}", NewsCommentsNumberPrefixCacheKey);

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        /// <remarks>
        /// {0} : news item ID
        /// </remarks>
        public static string NewsCommentsNumberPrefixCacheKey => "Nop.news.comments.number-{0}";

        #endregion
    }
}