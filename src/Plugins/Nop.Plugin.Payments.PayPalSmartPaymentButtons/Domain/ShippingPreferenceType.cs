namespace Nop.Plugin.Payments.PayPalSmartPaymentButtons.Domain
{
    /// <summary>
    /// Represents the type of shipping preference for customer checkout flow
    /// </summary>
    public enum ShippingPreferenceType
    {
        /// <summary>
        /// Use the customer-provided shipping address on the PayPal site.
        /// </summary>
        Get_from_file,

        /// <summary>
        /// Redact the shipping address from the PayPal site. Recommended for digital goods.
        /// </summary>
        No_shipping,

        /// <summary>
        /// Use the merchant-provided address. The customer cannot change this address on the PayPal site.
        /// </summary>
        Set_provided_address
    }
}