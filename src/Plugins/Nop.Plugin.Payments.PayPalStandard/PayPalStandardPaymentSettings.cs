using Nop.Core.Configuration;

namespace Nop.Plugin.Payments.PayPalStandard
{
    public class PayPalStandardPaymentSettings : ISettings
    {
        public bool UseSandbox { get; set; }
        public string BusinessEmail { get; set; }
        public string PdtToken { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether to "additional fee" is specified as percentage. true - percentage, false - fixed value.
        /// </summary>
        public bool AdditionalFeePercentage { get; set; }
        /// <summary>
        /// Additional fee
        /// </summary>
        public decimal AdditionalFee { get; set; }
        public bool PassProductNamesAndTotals { get; set; }
        public bool PdtValidateOrderTotal { get; set; }
        public bool EnableIpn { get; set; }
        public string IpnUrl { get; set; }
        /// <summary>
        /// Enable if a customer should be redirected to the order details page
        /// when he clicks "return to store" link on PayPal site
        /// WITHOUT completing a payment
        /// </summary>
        public bool ReturnFromPayPalWithoutPaymentRedirectsToOrderDetailsPage { get; set; }
        /// <summary>
        /// Enable PayPal address override
        /// </summary>
        public bool AddressOverride { get; set; }
    }
}
