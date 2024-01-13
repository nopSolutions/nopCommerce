using Nop.Core.Domain.Orders;

namespace Nop.Services.Payments;

/// <summary>
/// Represents a VoidPaymentResult
/// </summary>
public partial class VoidPaymentRequest
{
    /// <summary>
    /// Gets or sets an order
    /// </summary>
    public Order Order { get; set; }
}