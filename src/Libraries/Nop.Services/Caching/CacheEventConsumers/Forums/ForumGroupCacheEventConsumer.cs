using Nop.Core.Domain.Forums;
using Nop.Services.Caching.CachingDefaults;

namespace Nop.Services.Caching.CacheEventConsumers.Forums
{
    /// <summary>
    /// Represents a forum group cache event consumer
    /// </summary>
    public partial class ForumGroupCacheEventConsumer : CacheEventConsumer<ForumGroup>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        protected override void ClearCache(ForumGroup entity)
        {
            Remove(NopForumCachingDefaults.ForumGroupAllCacheKey);
            var cacheKey = NopForumCachingDefaults.ForumAllByForumGroupIdCacheKey.ToCacheKey(entity);
            Remove(cacheKey);
        }
    }
}
