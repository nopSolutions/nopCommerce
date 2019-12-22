using Nop.Core.Domain.Orders;
using Nop.Services.Caching.CachingDefaults;

namespace Nop.Services.Caching.CacheEventConsumers.Orders
{
    public partial class OrderCacheEventConsumer : CacheEventConsumer<Order>
    {
        public override void ClearCashe(Order entity)
        {
            _cacheManager.RemoveByPrefix(NopCatalogCachingDefaults.ProductPricePrefixCacheKey);
        }
    }
}