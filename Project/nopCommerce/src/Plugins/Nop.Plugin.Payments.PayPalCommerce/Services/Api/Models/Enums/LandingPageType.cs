namespace Nop.Plugin.Payments.PayPalCommerce.Services.Api.Models.Enums;

/// <summary>
/// Represents the type of the landing page
/// </summary>
public enum LandingPageType
{
    /// <summary>
    /// When the customer clicks PayPal Checkout, the customer is redirected to a page to log in to PayPal and approve the payment.
    /// </summary>
    LOGIN,

    /// <summary>
    /// When the customer clicks PayPal Checkout, the customer is redirected to a page to enter credit or debit card and other relevant billing information required to complete the purchase. This option has previously been also called as 'BILLING'
    /// </summary>
    GUEST_CHECKOUT,

    /// <summary>
    /// When the customer clicks PayPal Checkout, the customer is redirected to either a page to log in to PayPal and approve the payment or to a page to enter credit or debit card and other relevant billing information required to complete the purchase, depending on their previous interaction with PayPal.
    /// </summary>
    NO_PREFERENCE
}