﻿using Nop.Core.Domain.Payments;

namespace Nop.Services.Payments;

/// <summary>
/// Process payment result
/// </summary>
public partial class ProcessPaymentResult : BaseNopResult
{
    /// <summary>
    /// Gets or sets an AVS result
    /// </summary>
    public string AvsResult { get; set; }

    /// <summary>
    /// Gets or sets an CVV2 result
    /// </summary>
    public string Cvv2Result { get; set; }

    /// <summary>
    /// Gets or sets the authorization transaction identifier
    /// </summary>
    public string AuthorizationTransactionId { get; set; }

    /// <summary>
    /// Gets or sets the authorization transaction code
    /// </summary>
    public string AuthorizationTransactionCode { get; set; }

    /// <summary>
    /// Gets or sets the authorization transaction result
    /// </summary>
    public string AuthorizationTransactionResult { get; set; }

    /// <summary>
    /// Gets or sets the capture transaction identifier
    /// </summary>
    public string CaptureTransactionId { get; set; }

    /// <summary>
    /// Gets or sets the capture transaction result
    /// </summary>
    public string CaptureTransactionResult { get; set; }

    /// <summary>
    /// Gets or sets the subscription transaction identifier
    /// </summary>
    public string SubscriptionTransactionId { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether storing of credit card number, CVV2 is allowed
    /// </summary>
    public bool AllowStoringCreditCardNumber { get; set; }

    /// <summary>
    /// Gets or sets a value indicating that the recurring payment failed
    /// </summary>
    public bool RecurringPaymentFailed { get; set; }

    /// <summary>
    /// Gets or sets a payment status after processing
    /// </summary>
    public PaymentStatus NewPaymentStatus { get; set; } = PaymentStatus.Pending;
}