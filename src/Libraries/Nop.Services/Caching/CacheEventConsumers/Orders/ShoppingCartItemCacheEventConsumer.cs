using Nop.Core.Domain.Orders;
using Nop.Services.Caching.CachingDefaults;

namespace Nop.Services.Caching.CacheEventConsumers.Orders
{
    /// <summary>
    /// Represents a shopping cart item
    /// </summary>
    public partial class ShoppingCartItemCacheEventConsumer : EntityCacheEventConsumer<ShoppingCartItem>
    {
        public override void ClearCashe(ShoppingCartItem entity)
        {
            _cacheManager.RemoveByPrefix(NopOrderCachingDefaults.ShoppingCartPrefixCacheKey);

            base.ClearCashe(entity);
        }
    }
}
