namespace Nop.Core.Domain.Orders;

/// <summary>
/// Order item created event
/// </summary>
public partial class OrderItemCreatedEvent
{
    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="orderItem">Order item</param>
    /// <param name="shoppingCartItem">Shopping cart item</param>
    public OrderItemCreatedEvent(ShoppingCartItem shoppingCartItem, OrderItem orderItem)
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