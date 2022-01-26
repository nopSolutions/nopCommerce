using Nop.Core.Configuration;

namespace Nop.Plugin.Payments.PayPalStandard
{
    /// <summary>
    /// Represents settings of the PayPal Standard payment plugin
    /// </summary>
    public class PayPalStandardPaymentSettings : ISettings
    {
        /// <summary>
        /// Gets or sets a value indicating whether to use sandbox (testing environment)
        /// </summary>
        public bool UseSandbox { get; set; }

        /// <summary>
        /// Gets or sets a business email
        /// </summary>
        public string BusinessEmail { get; set; }

        /// <summary>
        /// Gets or sets PDT identity token
        /// </summary>
        public string PdtToken { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to pass info about purchased items to PayPal
        /// </summary>
        public bool PassProductNamesAndTotals { get; set; }

        /// <summary>
        /// Gets or sets an additional fixed fee
        /// </summary>
        public decimal AdditionalFeeFixed { get; set; }

        /// <summary>
        /// Gets or sets an additional fee by percentage
        /// </summary>
        public decimal AdditionalFeePercentage { get; set; }
    }
}