using Nop.Core.Domain.Shipping;
using Nop.Services.Caching.CachingDefaults;

namespace Nop.Services.Caching.CacheEventConsumers.Shipping
{
    public partial class WarehouseCacheEventConsumer : CacheEventConsumer<Warehouse>
    {
        public override void ClearCashe(Warehouse entity)
        {
            _cacheManager.RemoveByPrefix(NopShippingCachingDefaults.WarehousesPrefixCacheKey);
        }
    }
}
