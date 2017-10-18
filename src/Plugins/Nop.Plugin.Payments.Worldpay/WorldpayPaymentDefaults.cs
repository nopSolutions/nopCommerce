
namespace Nop.Plugin.Payments.Worldpay
{
    /// <summary>
    /// Represents constants of the Worldpay payment plugin
    /// </summary>
    public static class WorldpayPaymentDefaults
    {
        /// <summary>
        /// Worldpay payment method system name
        /// </summary>
        public const string SystemName = "Payments.WorldpayUS";

        /// <summary>
        /// User agent used for requesting Worldpay services
        /// </summary>
        public const string UserAgent = "nopCommerce-plugin-3.0";

        /// <summary>
        /// Key of the attribute to store Worldpay Vault customer identifier
        /// </summary>
        public const string CustomerIdAttribute = "WorldpayCustomerId";
    }
}