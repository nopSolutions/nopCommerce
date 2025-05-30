namespace Nop.Plugin.Payments.PayPalCommerce.Services.Api.Models.Enums;

/// <summary>
/// Represents the type of the authorization status
/// </summary>
public enum AuthorizationStatusType
{
    /// <summary>
    /// The authorized payment is created. No captured payments have been made for this authorized payment.
    /// </summary>
    CREATED,

    /// <summary>
    /// The authorized payment has one or more captures against it. The sum of these captured payments is greater than the amount of the original authorized payment.
    /// </summary>
    CAPTURED,

    /// <summary>
    /// PayPal cannot authorize funds for this authorized payment.
    /// </summary>
    DENIED,

    /// <summary>
    /// A captured payment was made for the authorized payment for an amount that is less than the amount of the original authorized payment.
    /// </summary>
    PARTIALLY_CAPTURED,

    /// <summary>
    /// The authorized payment was voided. No more captured payments can be made against this authorized payment.
    /// </summary>
    VOIDED,

    /// <summary>
    /// The created authorization is in pending state. For more information, see status.details.
    /// </summary>
    PENDING
}