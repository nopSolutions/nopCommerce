using Nop.Core.Domain.Shipping;
using Nop.Services.Caching.CachingDefaults;

namespace Nop.Services.Caching.CacheEventConsumers.Shipping
{
    public partial class WarehouseCacheEventConsumer : CacheEventConsumer<Warehouse>
    {
        public override void ClearCache(Warehouse entity)
        {
            RemoveByPrefix(NopShippingCachingDefaults.WarehousesPrefixCacheKey);
        }
    }
}
