using Nop.Core.Domain.Forums;
using Nop.Services.Caching.CachingDefaults;

namespace Nop.Services.Caching.CacheEventConsumers.Forums
{
    /// <summary>
    /// Represents a forum post cache event consumer
    /// </summary>
    public partial class ForumPostCacheEventConsumer : CacheEventConsumer<ForumPost>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        protected override void ClearCache(ForumPost entity)
        {
            RemoveByPrefix(NopForumCachingDefaults.ForumGroupPrefixCacheKey);
            RemoveByPrefix(NopForumCachingDefaults.ForumPrefixCacheKey);
        }
    }
}
