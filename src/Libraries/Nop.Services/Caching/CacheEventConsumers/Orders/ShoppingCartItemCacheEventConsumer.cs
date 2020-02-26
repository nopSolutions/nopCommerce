using Nop.Core.Domain.Orders;
using Nop.Services.Caching.CachingDefaults;

namespace Nop.Services.Caching.CacheEventConsumers.Orders
{
    /// <summary>
    /// Represents a shopping cart item cache event consumer
    /// </summary>
    public partial class ShoppingCartItemCacheEventConsumer : CacheEventConsumer<ShoppingCartItem>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        protected override void ClearCache(ShoppingCartItem entity)
        {
            RemoveByPrefix(NopOrderCachingDefaults.ShoppingCartPrefixCacheKey, false);
        }
    }
}
