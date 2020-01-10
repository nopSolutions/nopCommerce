using Nop.Core.Domain.Topics;
using Nop.Services.Caching.CachingDefaults;

namespace Nop.Services.Caching.CacheEventConsumers.Topics
{
    public partial class TopicCacheEventConsumer : CacheEventConsumer<Topic>
    {
        protected override void ClearCache(Topic entity)
        {
            RemoveByPrefix(NopTopicCachingDefaults.TopicsPrefixCacheKey);
        }
    }
}
