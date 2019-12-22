using Nop.Core.Domain.Topics;
using Nop.Services.Caching.CachingDefaults;

namespace Nop.Services.Caching.CacheEventConsumers.Topics
{
    public partial class TopicCacheEventConsumer : CacheEventConsumer<Topic>
    {
        public override void ClearCashe(Topic entity)
        {
            _cacheManager.RemoveByPrefix(NopTopicCachingDefaults.TopicsPrefixCacheKey);
        }
    }
}
