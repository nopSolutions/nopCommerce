using Nop.Core.Domain.Topics;
using Nop.Services.Caching.CachingDefaults;

namespace Nop.Services.Caching.CacheEventConsumers.Topics
{
    /// <summary>
    /// Represents a topic template cache event consumer
    /// </summary>
    public partial class TopicTemplateCacheEventConsumer : CacheEventConsumer<TopicTemplate>
    {
        /// <summary>
        /// entity
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <param name="entityEventType">Entity event type</param>
        protected override void ClearCache(TopicTemplate entity, EntityEventType entityEventType)
        {
            RemoveByPrefix(NopTopicCachingDefaults.TopicTemplatesPrefixCacheKey);
        }
    }
}
