using Nop.Core.Domain.Common;
using Nop.Services.Caching.CachingDefaults;

namespace Nop.Services.Caching.CacheEventConsumers.Common
{
    public partial class AddressAttributeCacheEventConsumer : CacheEventConsumer<AddressAttribute>
    {
        public override void ClearCache(AddressAttribute entity)
        {
            RemoveByPrefix(NopCommonCachingDefaults.AddressAttributesPrefixCacheKey);
            RemoveByPrefix(NopCommonCachingDefaults.AddressAttributeValuesPrefixCacheKey);
        }
    }
}
