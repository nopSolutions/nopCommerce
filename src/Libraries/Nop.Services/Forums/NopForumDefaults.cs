using Nop.Core.Caching;

namespace Nop.Services.Forums
{
    /// <summary>
    /// Represents default values related to forums services
    /// </summary>
    public static partial class NopForumDefaults
    {
        #region Caching defaults

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        public static CacheKey ForumGroupAllCacheKey => new CacheKey("Nop.forumgroup.all");

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : forum group ID
        /// </remarks>
        public static CacheKey ForumAllByForumGroupIdCacheKey => new CacheKey("Nop.forum.allbyforumgroupid-{0}");

        #endregion
    }
}