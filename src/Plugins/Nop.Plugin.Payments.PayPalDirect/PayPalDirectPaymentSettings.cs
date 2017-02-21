using Nop.Core.Configuration;

namespace Nop.Plugin.Payments.PayPalDirect
{
    /// <summary>
    /// PayPal Direct payment settings
    /// </summary>
    public class PayPalDirectPaymentSettings : ISettings
    {
        /// <summary>
        /// Gets or sets the client id
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// Gets or sets the secret key
        /// </summary>
        public string ClientSecret { get; set; }

        /// <summary>
        /// Gets or sets the transaction mode
        /// </summary>
        public TransactMode TransactMode { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to use sandbox (testing environment)
        /// </summary>
        public bool UseSandbox { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to pass info about purchased items to PayPal
        /// </summary>
        public bool PassPurchasedItems{ get; set; }

        /// <summary>
        /// Gets or sets an additional fee
        /// </summary>
        public decimal AdditionalFee { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to "additional fee" is specified as percentage. true - percentage, false - fixed value.
        /// </summary>
        public bool AdditionalFeePercentage { get; set; }

        /// <summary>
        /// Gets or sets a webhook id
        /// </summary>
        public string WebhookId { get; set; }
    }
}
