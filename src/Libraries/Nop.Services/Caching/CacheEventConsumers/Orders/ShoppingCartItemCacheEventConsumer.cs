using Nop.Core.Domain.Orders;

namespace Nop.Services.Caching.CacheEventConsumers.Orders
{
    /// <summary>
    /// Represents a shopping cart item
    /// </summary>
    public partial class ShoppingCartItemCacheEventConsumer : EntityCacheEventConsumer<ShoppingCartItem>
    {
    }
}
