using Nop.Core.Domain.Payments;

namespace Nop.Services.Payments;

/// <summary>
/// Capture payment result
/// </summary>
public partial class CapturePaymentResult : BaseNopResult
{
    /// <summary>
    /// Gets or sets the capture transaction identifier
    /// </summary>
    public string CaptureTransactionId { get; set; }

    /// <summary>
    /// Gets or sets the capture transaction result
    /// </summary>
    public string CaptureTransactionResult { get; set; }

    /// <summary>
    /// Gets or sets a payment status after processing
    /// </summary>
    public PaymentStatus NewPaymentStatus { get; set; } = PaymentStatus.Pending;
}