using Nop.Core.Domain.Forums;
using Nop.Services.Caching.CachingDefaults;

namespace Nop.Services.Caching.CacheEventConsumers.Forums
{
    public partial class ForumPostCacheEventConsumer : CacheEventConsumer<ForumPost>
    {
        public override void ClearCache(ForumPost entity)
        {
            RemoveByPrefix(NopForumCachingDefaults.ForumGroupPrefixCacheKey);
            RemoveByPrefix(NopForumCachingDefaults.ForumPrefixCacheKey);
        }
    }
}
