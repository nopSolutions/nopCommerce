using Nop.Core.Domain.Payments;

namespace Nop.Services.Payments;

/// <summary>
/// Refund payment result
/// </summary>
public partial class RefundPaymentResult : BaseNopResult
{
    /// <summary>
    /// Gets or sets a payment status after processing
    /// </summary>
    public PaymentStatus NewPaymentStatus { get; set; } = PaymentStatus.Pending;
}