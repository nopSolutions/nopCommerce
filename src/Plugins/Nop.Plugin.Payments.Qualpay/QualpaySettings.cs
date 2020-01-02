using Nop.Core.Configuration;
using Nop.Plugin.Payments.Qualpay.Domain;

namespace Nop.Plugin.Payments.Qualpay
{
    /// <summary>
    /// Represents Qualpay payment gateway settings
    /// </summary>
    public class QualpaySettings : ISettings
    {
        /// <summary>
        /// Gets or sets a merchant identifier
        /// </summary>
        public string MerchantId { get; set; }

        /// <summary>
        /// Gets or sets a profile identifier
        /// </summary>
        public string ProfileId { get; set; }

        /// <summary>
        /// Gets or sets a security key
        /// </summary>
        public string SecurityKey { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to use sandbox (testing environment)
        /// </summary>
        public bool UseSandbox { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to use Qualpay Embedded Fields feature
        /// </summary>
        public bool UseEmbeddedFields { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to use Qualpay Customer Vault feature
        /// </summary>
        public bool UseCustomerVault { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to use Qualpay Recurring Billing feature
        /// </summary>
        public bool UseRecurringBilling { get; set; }

        /// <summary>
        /// Gets or sets the payment transaction type (authorization only or authorization and capture in a single request)
        /// </summary>
        public TransactionType PaymentTransactionType { get; set; }

        /// <summary>
        /// Gets or sets an additional fee
        /// </summary>
        public decimal AdditionalFee { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to "additional fee" is specified as percentage. true - percentage, false - fixed value.
        /// </summary>
        public bool AdditionalFeePercentage { get; set; }

        /// <summary>
        /// Gets or sets a webhook identifier
        /// </summary>
        public string WebhookId { get; set; }

        /// <summary>
        /// Gets or sets a webhook secret key
        /// </summary>
        public string WebhookSecretKey { get; set; }

        /// <summary>
        /// Gets or sets a merchant email (used to subscribe for Qualpay news)
        /// </summary>
        public string MerchantEmail { get; set; }
    }
}