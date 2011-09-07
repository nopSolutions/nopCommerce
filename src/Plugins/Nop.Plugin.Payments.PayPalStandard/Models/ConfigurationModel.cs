using System.ComponentModel;
using System.Web.Mvc;
using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.Payments.PayPalStandard.Models
{
    public class ConfigurationModel : BaseNopModel
    {
        [DisplayName("Use Sandbox")]
        public bool UseSandbox { get; set; }

        [DisplayName("Business Email")]
        public string BusinessEmail { get; set; }

        [DisplayName("PDT Identity Token")]
        public string PdtToken { get; set; }

        [DisplayName("PDT. Validate order total")]
        public bool PdtValidateOrderTotal { get; set; }

        [DisplayName("Additional fee")]
        public decimal AdditionalFee { get; set; }

        [DisplayName("Pass product names and order totals to PayPal")]
        public bool PassProductNamesAndTotals { get; set; }

        [DisplayName("Override the URL of IPN handler (optional)")]
        public string IpnUrl { get; set; }
    }
}