namespace Nop.Core.Domain.Orders;

/// <summary>
/// Order voided event
/// </summary>
public partial class OrderVoidedEvent
{
    #region Ctor

    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="order">Order</param>
    public OrderVoidedEvent(Order order)
    {
        Order = order;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Voided order
    /// </summary>
    public Order Order { get; }

    #endregion
}