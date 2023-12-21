namespace Nop.Plugin.Payments.PayPalCommerce.Domain;

/// <summary>
/// Represents payment type enumeration
/// </summary>
public enum PaymentType
{
    /// <summary>
    /// The merchant intends to capture payment immediately after the customer makes a payment.
    /// </summary>
    Capture,

    /// <summary>
    /// The merchant intends to authorize a payment and place funds on hold after the customer makes a payment. 
    /// Authorized payments are guaranteed for up to three days but are available to capture for up to 29 days.
    /// After the three-day honor period, the original authorized payment expires and you must re-authorize the payment.
    /// </summary>
    Authorize
}