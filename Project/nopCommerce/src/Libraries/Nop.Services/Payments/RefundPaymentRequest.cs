using Nop.Core.Domain.Orders;

namespace Nop.Services.Payments;

/// <summary>
/// Represents a RefundPaymentResult
/// </summary>
public partial class RefundPaymentRequest
{
    /// <summary>
    /// Gets or sets an order
    /// </summary>
    public Order Order { get; set; }

    /// <summary>
    /// Gets or sets an amount
    /// </summary>
    public decimal AmountToRefund { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether it's a partial refund; otherwise, full refund
    /// </summary>
    public bool IsPartialRefund { get; set; }
}