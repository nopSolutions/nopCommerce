using Nop.Core.Domain.Forums;
using Nop.Services.Caching.CachingDefaults;

namespace Nop.Services.Caching.CacheEventConsumers.Forums
{
    public partial class ForumTopicCacheEventConsumer : CacheEventConsumer<ForumTopic>
    {
        public override void ClearCashe(ForumTopic entity)
        {
            _cacheManager.RemoveByPrefix(NopForumCachingDefaults.ForumGroupPrefixCacheKey);
            _cacheManager.RemoveByPrefix(NopForumCachingDefaults.ForumPrefixCacheKey);
        }
    }
}
