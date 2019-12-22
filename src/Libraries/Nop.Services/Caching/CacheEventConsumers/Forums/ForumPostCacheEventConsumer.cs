using Nop.Core.Domain.Forums;
using Nop.Services.Caching.CachingDefaults;

namespace Nop.Services.Caching.CacheEventConsumers.Forums
{
    public partial class ForumPostCacheEventConsumer : CacheEventConsumer<ForumPost>
    {
        public override void ClearCashe(ForumPost entity)
        {
            _cacheManager.RemoveByPrefix(NopForumCachingDefaults.ForumGroupPrefixCacheKey);
            _cacheManager.RemoveByPrefix(NopForumCachingDefaults.ForumPrefixCacheKey);
        }
    }
}
