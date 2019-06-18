using Nop.Core.Configuration;

namespace Nop.Plugin.Payments.MercadoPago
{
    /// <summary>
    /// PayPal Direct payment settings
    /// </summary>
    public class MercadoPagoPaymentSettings : ISettings
    {
        public string PublicKey { get; set; }

        public string AccessToken { get; set; }

        public string PublicKeySandbox { get; set; }

        public string AccessTokenSandbox { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to use sandbox (testing environment)
        /// </summary>
        public bool UseSandbox { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to pass info about purchased items to Mercado Pago
        /// </summary>
        public bool PassPurchasedItems { get; set; }

        public string PaymentMethodDescription { get; set; }
    }
}
