using Nop.Core.Configuration;
using Nop.Plugin.Payments.Worldpay.Domain;

namespace Nop.Plugin.Payments.Worldpay
{
    /// <summary>
    /// Represents a Worldpay payment settings
    /// </summary>
    public class WorldpayPaymentSettings : ISettings
    {
        /// <summary>
        /// Gets or sets a SecureNet identifier
        /// </summary>
        public string SecureNetId { get; set; }

        /// <summary>
        /// Gets or sets a secure key
        /// </summary>
        public string SecureKey { get; set; }

        /// <summary>
        /// Gets or sets a public key
        /// </summary>
        public string PublicKey { get; set; }

        /// <summary>
        /// Gets or sets the transaction mode
        /// </summary>
        public TransactionMode TransactionMode { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to use sandbox (testing environment)
        /// </summary>
        public bool UseSandbox { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to validate billing address
        /// </summary>
        public bool ValidateAddress { get; set; }

        /// <summary>
        /// Gets or sets an additional fee
        /// </summary>
        public decimal AdditionalFee { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to "additional fee" is specified as percentage
        /// </summary>
        public bool AdditionalFeePercentage { get; set; }
    }
}