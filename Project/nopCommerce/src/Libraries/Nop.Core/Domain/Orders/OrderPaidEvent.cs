namespace Nop.Core.Domain.Orders;

/// <summary>
/// Order paid event
/// </summary>
public partial class OrderPaidEvent
{
    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="order">Order</param>
    public OrderPaidEvent(Order order)
    {
        Order = order;
    }

    /// <summary>
    /// Order
    /// </summary>
    public Order Order { get; }
}