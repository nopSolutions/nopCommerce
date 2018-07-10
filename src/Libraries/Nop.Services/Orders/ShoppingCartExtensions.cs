using System.Collections.Generic;
using System.Linq;
using Nop.Core.Domain.Orders;
using Nop.Core.Infrastructure;

namespace Nop.Services.Orders
{
    /// <summary>
    /// Represents a shopping cart
    /// </summary>
    public static class ShoppingCartExtensions
    {
        /// <summary>
        /// Limit cart by store (if carts are not shared between stores)
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <param name="storeId">Store identifier</param>
        /// <returns>Cart</returns>
        public static IEnumerable<ShoppingCartItem> LimitPerStore(this IEnumerable<ShoppingCartItem> cart, int storeId)
        {
            var shoppingCartSettings = EngineContext.Current.Resolve<ShoppingCartSettings>();
            if (shoppingCartSettings.CartsSharedBetweenStores)
                return cart;

            return cart.Where(x => x.StoreId == storeId);
        }
    }
}