using Nop.Core.Domain.Orders;
using Nop.Services.Caching.CachingDefaults;

namespace Nop.Services.Caching.CacheEventConsumers.Orders
{
    /// <summary>
    /// Represents a order cache event consumer
    /// </summary>
    public partial class OrderCacheEventConsumer : CacheEventConsumer<Order>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        protected override void ClearCache(Order entity)
        {
            RemoveByPrefix(NopCatalogCachingDefaults.ProductPricePrefixCacheKey);
        }
    }
}