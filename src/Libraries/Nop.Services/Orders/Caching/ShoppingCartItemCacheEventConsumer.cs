using Nop.Core.Domain.Orders;
using Nop.Services.Caching;

namespace Nop.Services.Orders.Caching
{
    /// <summary>
    /// Represents a shopping cart item cache event consumer
    /// </summary>
    public partial class ShoppingCartItemCacheEventConsumer : CacheEventConsumer<ShoppingCartItem>
    {
    }
}
