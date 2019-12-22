using Nop.Core.Domain.Forums;
using Nop.Services.Caching.CachingDefaults;

namespace Nop.Services.Caching.CacheEventConsumers.Forums
{
    public partial class ForumGroupCacheEventConsumer : CacheEventConsumer<ForumGroup>
    {
        public override void ClearCashe(ForumGroup entity)
        {
            _cacheManager.RemoveByPrefix(NopForumCachingDefaults.ForumGroupPrefixCacheKey);
            _cacheManager.RemoveByPrefix(NopForumCachingDefaults.ForumPrefixCacheKey);
        }
    }
}
