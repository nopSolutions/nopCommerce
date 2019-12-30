using Nop.Core.Domain.Common;
using Nop.Services.Caching.CachingDefaults;

namespace Nop.Services.Caching.CacheEventConsumers.Common
{
    public partial class GenericAttributeCacheEventConsumer : CacheEventConsumer<GenericAttribute>
    {
        public override void ClearCache(GenericAttribute entity)
        {
            RemoveByPrefix(NopCommonCachingDefaults.GenericAttributePrefixCacheKey);
        }
    }
}
