using Nop.Core.Domain.Topics;
using Nop.Services.Caching;

namespace Nop.Services.Topics.Caching
{
    /// <summary>
    /// Represents a topic cache event consumer
    /// </summary>
    public partial class TopicCacheEventConsumer : CacheEventConsumer<Topic>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        protected override void ClearCache(Topic entity)
        {
            RemoveByPrefix(NopTopicDefaults.TopicsAllPrefixCacheKey);
            var prefix = _cacheKeyService.PrepareKeyPrefix(NopTopicDefaults.TopicBySystemNamePrefixCacheKey, entity.SystemName);
            RemoveByPrefix(prefix);
        }
    }
}
