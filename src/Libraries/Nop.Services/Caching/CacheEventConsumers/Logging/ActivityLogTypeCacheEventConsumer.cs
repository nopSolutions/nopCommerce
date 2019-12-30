using Nop.Core.Domain.Logging;
using Nop.Services.Caching.CachingDefaults;

namespace Nop.Services.Caching.CacheEventConsumers.Logging
{
    public partial class ActivityLogTypeCacheEventConsumer : CacheEventConsumer<ActivityLogType>
    {
        public override void ClearCache(ActivityLogType entity)
        {
            RemoveByPrefix(NopLoggingCachingDefaults.ActivityTypePrefixCacheKey);
        }
    }
}
