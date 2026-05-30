namespace Nop.Plugin.Payments.PayPalCommerce.Services.Api.Models.Enums;

/// <summary>
/// Represents the type of the stored payment usage
/// </summary>
public enum StoredPaymentUsageType
{
    /// <summary>
    /// Indicates the Initial/First payment with a payment_source that is intended to be stored upon successful processing of the payment.
    /// </summary>
    FIRST,

    /// <summary>
    /// Indicates a payment using a stored payment_source which has been successfully used previously for a payment.
    /// </summary>
    SUBSEQUENT,

    /// <summary>
    /// Indicates that PayPal will derive the value of `FIRST` or `SUBSEQUENT` based on data available to PayPal.
    /// </summary>
    DERIVED
}