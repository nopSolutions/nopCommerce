namespace Nop.Services.Forums
{
    /// <summary>
    /// Represents default values related to forums services
    /// </summary>
    public static partial class NopForumDefaults
    {
        /// <summary>
        /// Gets a key for caching
        /// </summary>
        public static string ForumGroupAllCacheKey => "Nop.forumgroup.all";

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : forum group ID
        /// </remarks>
        public static string ForumAllByForumGroupIdCacheKey => "Nop.forum.allbyforumgroupid-{0}";

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string ForumGroupPatternCacheKey => "Nop.forumgroup.";

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string ForumPatternCacheKey => "Nop.forum.";
    }
}