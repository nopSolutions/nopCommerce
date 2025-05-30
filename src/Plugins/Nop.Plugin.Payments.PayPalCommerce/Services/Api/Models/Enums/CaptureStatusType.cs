namespace Nop.Plugin.Payments.PayPalCommerce.Services.Api.Models.Enums;

/// <summary>
/// Represents the type of the capture status
/// </summary>
public enum CaptureStatusType
{
    /// <summary>
    /// The funds for this captured payment were credited to the payee's PayPal account.
    /// </summary>
    COMPLETED,

    /// <summary>
    /// The funds could not be captured.
    /// </summary>
    DECLINED,

    /// <summary>
    /// An amount less than this captured payment's amount was partially refunded to the payer.
    /// </summary>
    PARTIALLY_REFUNDED,

    /// <summary>
    /// The funds for this captured payment was not yet credited to the payee's PayPal account. For more information, see status.details.
    /// </summary>
    PENDING,

    /// <summary>
    /// An amount greater than or equal to this captured payment's amount was refunded to the payer.
    /// </summary>
    REFUNDED,

    /// <summary>
    /// There was an error while capturing payment.
    /// </summary>
    FAILED
}