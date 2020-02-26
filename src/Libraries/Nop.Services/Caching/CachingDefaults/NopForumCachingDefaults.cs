namespace Nop.Services.Caching.CachingDefaults
{
    /// <summary>
    /// Represents default values related to forums services
    /// </summary>
    public static partial class NopForumCachingDefaults
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
    }
}