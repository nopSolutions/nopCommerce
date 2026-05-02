using Nop.Core.Domain.Orders;

namespace Nop.Services.Orders;

/// <summary>
/// Place order result
/// </summary>
public partial class PlaceOrderResult : BaseNopResult
{
    /// <summary>
    /// Gets or sets the placed order
    /// </summary>
    public Order PlacedOrder { get; set; }
}