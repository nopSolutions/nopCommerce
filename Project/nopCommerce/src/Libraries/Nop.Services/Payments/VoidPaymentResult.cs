using Nop.Core.Domain.Payments;

namespace Nop.Services.Payments;

/// <summary>
/// Represents a VoidPaymentResult
/// </summary>
public partial class VoidPaymentResult : BaseNopResult
{
    /// <summary>
    /// Gets or sets a payment status after processing
    /// </summary>
    public PaymentStatus NewPaymentStatus { get; set; } = PaymentStatus.Pending;
}