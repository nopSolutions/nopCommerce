namespace Nop.Core.Domain.Orders
{
    /// <summary>
    /// Shopping cart cleared event
    /// </summary>
    public partial class ClearShoppingCartEvent
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cart">Shopping cart</param>
        public ClearShoppingCartEvent(IList<ShoppingCartItem> cart)
        {
            Cart = cart;
        }

        /// <summary>
        /// Shopping cart
        /// </summary>
        public IList<ShoppingCartItem> Cart { get; }
    }
}
