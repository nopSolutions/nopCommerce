using Nop.Core.Domain.Orders;

namespace Nop.Services.Payments;

/// <summary>
/// Represents a CapturePaymentRequest
/// </summary>
public partial class CapturePaymentRequest
{
    /// <summary>
    /// Gets or sets an order
    /// </summary>
    public Order Order { get; set; }
}