namespace Nop.Plugin.Payments.PayPalCommerce.Services.Api.Models.Enums;

/// <summary>
/// Represents the type of the seller protection
/// </summary>
public enum SellerProtectionType
{
    /// <summary>
    /// Your PayPal balance remains intact if the customer claims that they did not receive an item or the account holder claims that they did not authorize the payment.
    /// </summary>
    ELIGIBLE,

    /// <summary>
    /// Your PayPal balance remains intact if the customer claims that they did not receive an item.
    /// </summary>
    PARTIALLY_ELIGIBLE,

    /// <summary>
    /// This transaction is not eligible for seller protection.
    /// </summary>
    NOT_ELIGIBLE
}