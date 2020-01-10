using Nop.Core.Domain.Orders;
using Nop.Services.Caching.CachingDefaults;

namespace Nop.Services.Caching.CacheEventConsumers.Orders
{
    /// <summary>
    /// Represents a shopping cart item
    /// </summary>
    public partial class ShoppingCartItemCacheEventConsumer : CacheEventConsumer<ShoppingCartItem>
    {
        protected override void ClearCache(ShoppingCartItem entity)
        {
            RemoveByPrefix(NopNewsCachingDefaults.ShoppingCartPrefixCacheKey, false);
        }
    }
}
