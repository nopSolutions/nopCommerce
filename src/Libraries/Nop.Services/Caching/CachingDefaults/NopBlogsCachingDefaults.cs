namespace Nop.Services.Caching.CachingDefaults
{
    /// <summary>
    /// Represents default values related to blogs services
    /// </summary>
    public static partial class NopBlogsCachingDefaults
    {
        /// <summary>
        /// Key for number of blog comments
        /// </summary>
        /// <remarks>
        /// {0} : blog post ID
        /// {1} : store ID
        /// {2} : are only approved comments?
        /// </remarks>
        public static string BlogCommentsNumberKey => "Nop.blog.comments.number-{0}-{1}-{2}";

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string BlogCommentsPrefixCacheKey => "Nop.blog.comments";

        /// <summary>
        /// Key for blog tag list model
        /// </summary>
        /// <remarks>
        /// {0} : language ID
        /// {1} : current store ID
        /// </remarks>
        public static string BlogTagsModelKey => "Nop.blog.tags-{0}-{1}";

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string BlogPrefixCacheKey => "Nop.blog.tags";
    }
}