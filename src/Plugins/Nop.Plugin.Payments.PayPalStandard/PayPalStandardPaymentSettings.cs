using Nop.Core.Configuration;

namespace Nop.Plugin.Payments.PayPalStandard
{
    public class PayPalStandardPaymentSettings : ISettings
    {
        public bool UseSandbox { get; set; }
        public string BusinessEmail { get; set; }
        public string PdtToken { get; set; }
        public decimal AdditionalFee { get; set; }
        public bool PassProductNamesAndTotals { get; set; }
        public bool PdtValidateOrderTotal { get; set; }
        public bool EnableIpn { get; set; }
        public string IpnUrl { get; set; }
    }
}
