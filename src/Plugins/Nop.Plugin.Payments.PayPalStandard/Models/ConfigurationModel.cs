using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.Payments.PayPalStandard.Models
{
    public class ConfigurationModel : BaseNopModel
    {
        [NopResourceDisplayName("Plugins.Payments.PayPalStandard.Fields.UseSandbox")]
        public bool UseSandbox { get; set; }

        [NopResourceDisplayName("Plugins.Payments.PayPalStandard.Fields.BusinessEmail")]
        public string BusinessEmail { get; set; }

        [NopResourceDisplayName("Plugins.Payments.PayPalStandard.Fields.PDTToken")]
        public string PdtToken { get; set; }

        [NopResourceDisplayName("Plugins.Payments.PayPalStandard.Fields.PDTValidateOrderTotal")]
        public bool PdtValidateOrderTotal { get; set; }

        [NopResourceDisplayName("Plugins.Payments.PayPalStandard.Fields.AdditionalFee")]
        public decimal AdditionalFee { get; set; }

        [NopResourceDisplayName("Plugins.Payments.PayPalStandard.Fields.PassProductNamesAndTotals")]
        public bool PassProductNamesAndTotals { get; set; }

        [NopResourceDisplayName("Plugins.Payments.PayPalStandard.Fields.EnableIpn")]
        public bool EnableIpn { get; set; }

        [NopResourceDisplayName("Plugins.Payments.PayPalStandard.Fields.IpnUrl")]
        public string IpnUrl { get; set; }
    }
}