using Nop.Core.Domain.Forums;
using Nop.Services.Caching.CachingDefaults;

namespace Nop.Services.Caching.CacheEventConsumers.Forums
{
    /// <summary>
    /// Represents a forum topic cache event consumer
    /// </summary>
    public partial class ForumTopicCacheEventConsumer : CacheEventConsumer<ForumTopic>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        protected override void ClearCache(ForumTopic entity)
        {
            RemoveByPrefix(NopForumCachingDefaults.ForumGroupPrefixCacheKey);
            RemoveByPrefix(NopForumCachingDefaults.ForumPrefixCacheKey);
        }
    }
}
