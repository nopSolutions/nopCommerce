namespace Nop.Plugin.Payments.PayPalCommerce.Services.Api.Models.Enums;

/// <summary>
/// Represents the type of the payment method preference
/// </summary>
public enum PaymentMethodPreferenceType
{
    /// <summary>
    /// Accepts any type of payment from the customer.
    /// </summary>
    UNRESTRICTED,

    /// <summary>
    /// Accepts only immediate payment from the customer. For example, credit card, PayPal balance, or instant ACH. Ensures that at the time of capture, the payment does not have the `pending` status.
    /// </summary>
    IMMEDIATE_PAYMENT_REQUIRED
}