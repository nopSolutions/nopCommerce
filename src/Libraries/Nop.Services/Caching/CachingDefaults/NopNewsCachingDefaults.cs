namespace Nop.Services.Caching.CachingDefaults
{
    /// <summary>
    /// Represents default values related to orders services
    /// </summary>
    public static partial class NopNewsCachingDefaults
    {
        /// <summary>
        /// Key for number of news comments
        /// </summary>
        /// <remarks>
        /// {0} : news item ID
        /// {1} : store ID
        /// {2} : are only approved comments?
        /// </remarks>
        public static string NewsCommentsNumberKey => "Nop.news.comments.number-{0}-{1}-{2}";

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string NewsCommentsPrefixCacheKey => "Nop.news.comments";
    }
}