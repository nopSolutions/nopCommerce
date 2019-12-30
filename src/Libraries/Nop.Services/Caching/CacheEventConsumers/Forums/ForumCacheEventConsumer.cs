using Nop.Core.Domain.Forums;
using Nop.Services.Caching.CachingDefaults;

namespace Nop.Services.Caching.CacheEventConsumers.Forums
{
    public partial class ForumCacheEventConsumer : CacheEventConsumer<Forum>
    {
        public override void ClearCache(Forum entity)
        {
            RemoveByPrefix(NopForumCachingDefaults.ForumGroupPrefixCacheKey);
            RemoveByPrefix(NopForumCachingDefaults.ForumPrefixCacheKey);
        }
    }
}
