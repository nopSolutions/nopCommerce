namespace Nop.Core.Domain.Orders;

/// <summary>
/// Order item created by shopping cart item event
/// </summary>
public partial class ShoppingCartItemMovedToOrderItemEvent
{
    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="orderItem">Order item</param>
    /// <param name="shoppingCartItem">Shopping cart item</param>
    public ShoppingCartItemMovedToOrderItemEvent(ShoppingCartItem shoppingCartItem, OrderItem orderItem)
    {
        ShoppingCartItem = shoppingCartItem;
        OrderItem = orderItem;
    }

    /// <summary>
    /// Gets shopping cart item
    /// </summary>
    public ShoppingCartItem ShoppingCartItem { get; }

    /// <summary>
    /// Gets order item
    /// </summary>
    public OrderItem OrderItem { get; }
}