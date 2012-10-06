using System.Web.Mvc;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.Payments.PayPalDirect.Models
{
    public class ConfigurationModel : BaseNopModel
    {
        [NopResourceDisplayName("Plugins.Payments.PayPalDirect.Fields.UseSandbox")]
        public bool UseSandbox { get; set; }

        public int TransactModeId { get; set; }
        [NopResourceDisplayName("Plugins.Payments.PayPalDirect.Fields.TransactMode")]
        public SelectList TransactModeValues { get; set; }

        [NopResourceDisplayName("Plugins.Payments.PayPalDirect.Fields.ApiAccountName")]
        public string ApiAccountName { get; set; }

        [NopResourceDisplayName("Plugins.Payments.PayPalDirect.Fields.ApiAccountPassword")]
        public string ApiAccountPassword { get; set; }

        [NopResourceDisplayName("Plugins.Payments.PayPalDirect.Fields.Signature")]
        public string Signature { get; set; }

        [NopResourceDisplayName("Plugins.Payments.PayPalDirect.Fields.AdditionalFee")]
        public decimal AdditionalFee { get; set; }

        [NopResourceDisplayName("Plugins.Payments.PayPalDirect.Fields.AdditionalFeePercentage")]
        public bool AdditionalFeePercentage { get; set; }
    }
}