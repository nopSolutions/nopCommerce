namespace Nop.Plugin.Payments.PayPalCommerce.Services.Api.Models.Enums;

/// <summary>
/// Represents the type of the payment token
/// </summary>
public enum PaymentTokenType
{
    /// <summary>
    /// The setup token, which is a temporary reference to payment source.
    /// </summary>
    SETUP_TOKEN,

    /// <summary>
    /// The PayPal billing agreement ID. References an approved recurring payment for goods or services.
    /// </summary>
    BILLING_AGREEMENT
}