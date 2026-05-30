namespace Nop.Plugin.Payments.PayPalCommerce.Services.Api.Models.Enums;

/// <summary>
/// Represents the type of the intent
/// </summary>
public enum IntentType
{
    /// <summary>
    /// The merchant intends to capture payment immediately after the customer makes a payment.
    /// </summary>
    CAPTURE,

    /// <summary>
    /// The merchant intends to authorize a payment and place funds on hold after the customer makes a payment. Authorized payments are best captured within three days of authorization but are available to capture for up to 29 days. After the three-day honor period, the original authorized payment expires and you must re-authorize the payment. You must make a separate request to capture payments on demand. This intent is not supported when you have more than one `purchase_unit` within your order.
    /// </summary>
    AUTHORIZE
}