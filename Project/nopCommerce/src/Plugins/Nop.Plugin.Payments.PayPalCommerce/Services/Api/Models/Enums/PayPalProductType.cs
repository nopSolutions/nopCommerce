namespace Nop.Plugin.Payments.PayPalCommerce.Services.Api.Models.Enums;

/// <summary>
/// Represents the type of the PayPal product
/// </summary>
public enum PayPalProductType
{
    /// <summary>
    /// Express checkout product
    /// </summary>
    EXPRESS_CHECKOUT,

    /// <summary>
    /// iDEAL is a Netherlands-based payment method that allows customers to complete transactions online using their bank credentials.
    /// </summary>
    IDEAL,

    /// <summary>
    /// PayPal PLUS product.
    /// </summary>
    PPPLUS,

    /// <summary>
    /// PayPal Professional product
    /// </summary>
    WEBSITE_PAYMENT_PRO,

    /// <summary>
    /// PayPal Alternative Payment Methods product
    /// </summary>
    PAYMENT_METHODS,

    /// <summary>
    /// PayPal Complete Payments product, which includes advanced debit and credit card payments, Apple Pay, and Google Pay.
    /// </summary>
    PPCP,

    /// <summary>
    /// PayPal Complete Payments product, which includes advanced debit and credit card payments, Apple Pay, and Google Pay.
    /// </summary>
    PPCP_CUSTOM,

    /// <summary>
    /// PayPal Complete Payments product, which includes advanced debit and credit card payments, Apple Pay, and Google Pay.
    /// </summary>
    PPCP_STANDARD,

    /// <summary>
    /// PayPal Advanced Vaulting product. Must be requested along with either EXPRESS_CHECKOUT or PPCP
    /// </summary>
    ADVANCED_VAULTING,

    /// <summary>
    /// PayPal Zettle in-person payments product
    /// </summary>
    ZETTLE
}