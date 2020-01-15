using Nop.Core.Domain.Forums;
using Nop.Services.Caching.CachingDefaults;

namespace Nop.Services.Caching.CacheEventConsumers.Forums
{
    /// <summary>
    /// Represents a forum cache event consumer
    /// </summary>
    public partial class ForumCacheEventConsumer : CacheEventConsumer<Forum>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        protected override void ClearCache(Forum entity)
        {
            RemoveByPrefix(NopForumCachingDefaults.ForumGroupPrefixCacheKey);
            RemoveByPrefix(NopForumCachingDefaults.ForumPrefixCacheKey);
        }
    }
}
