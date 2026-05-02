namespace Nop.Plugin.Payments.PayPalCommerce.Services.Api.Models.Enums;

/// <summary>
/// Represents the type of the shipping preference
/// </summary>
public enum ShippingPreferenceType
{
    /// <summary>
    /// Get the customer-provided shipping address on the PayPal site.
    /// </summary>
    GET_FROM_FILE,

    /// <summary>
    /// Removes the shipping address information from the API response and the Paypal site. However, the shipping.phone_number and shipping.email_address fields will still be returned to allow for digital goods delivery.
    /// </summary>
    NO_SHIPPING,

    /// <summary>
    /// Get the merchant-provided address. The customer cannot change this address on the PayPal site. If merchant does not pass an address, customer can choose the address on PayPal pages.
    /// </summary>
    SET_PROVIDED_ADDRESS
}