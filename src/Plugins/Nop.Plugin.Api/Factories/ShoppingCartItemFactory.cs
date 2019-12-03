using System;
using Nop.Core.Domain.Orders;

namespace Nop.Plugin.Api.Factories
{
    public class ShoppingCartItemFactory : IFactory<ShoppingCartItem>
    {
        public ShoppingCartItem Initialize()
        {
            var newShoppingCartItem = new ShoppingCartItem();

            newShoppingCartItem.CreatedOnUtc = DateTime.UtcNow;
            newShoppingCartItem.UpdatedOnUtc = DateTime.UtcNow;

            return newShoppingCartItem;
        }
    }
}