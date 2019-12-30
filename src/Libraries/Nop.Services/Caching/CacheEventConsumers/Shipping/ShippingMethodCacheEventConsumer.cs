using Nop.Core.Domain.Shipping;
using Nop.Services.Caching.CachingDefaults;

namespace Nop.Services.Caching.CacheEventConsumers.Shipping
{
    public partial class ShippingMethodCacheEventConsumer : CacheEventConsumer<ShippingMethod>
    {
        public override void ClearCache(ShippingMethod entity)
        {
            RemoveByPrefix(NopShippingCachingDefaults.ShippingMethodsPrefixCacheKey);
        }
    }
}
