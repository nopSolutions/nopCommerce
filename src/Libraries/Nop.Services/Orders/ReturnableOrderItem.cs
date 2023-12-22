using Nop.Core.Domain.Orders;

namespace Nop.Services.Orders;

/// <summary>
/// Represents the returnable order item
/// </summary>
public partial class ReturnableOrderItem
{
    #region Properties

    /// <summary>
    /// Gets or sets the available quantity for return
    /// </summary>
    public int AvailableQuantityForReturn { get; set; }

    /// <summary>
    /// Gets or sets the order item for return
    /// </summary>
    public OrderItem OrderItem { get; set; }

    #endregion
}