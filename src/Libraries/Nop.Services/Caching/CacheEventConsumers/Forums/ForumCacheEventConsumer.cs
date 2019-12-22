using Nop.Core.Domain.Forums;
using Nop.Services.Caching.CachingDefaults;

namespace Nop.Services.Caching.CacheEventConsumers.Forums
{
    public partial class ForumCacheEventConsumer : CacheEventConsumer<Forum>
    {
        public override void ClearCashe(Forum entity)
        {
            _cacheManager.RemoveByPrefix(NopForumCachingDefaults.ForumGroupPrefixCacheKey);
            _cacheManager.RemoveByPrefix(NopForumCachingDefaults.ForumPrefixCacheKey);
        }
    }
}
