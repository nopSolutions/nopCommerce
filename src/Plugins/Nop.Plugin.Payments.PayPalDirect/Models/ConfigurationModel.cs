using System.Web.Mvc;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.Payments.PayPalDirect.Models
{
    public class ConfigurationModel : BaseNopModel
    {
        public int ActiveStoreScopeConfiguration { get; set; }

        [NopResourceDisplayName("Plugins.Payments.PayPalDirect.Fields.UseSandbox")]
        public bool UseSandbox { get; set; }
        public bool UseSandbox_OverrideForStore { get; set; }

        public int TransactModeId { get; set; }
        public bool TransactModeId_OverrideForStore { get; set; }
        [NopResourceDisplayName("Plugins.Payments.PayPalDirect.Fields.TransactMode")]
        public SelectList TransactModeValues { get; set; }

        [NopResourceDisplayName("Plugins.Payments.PayPalDirect.Fields.ApiAccountName")]
        public string ApiAccountName { get; set; }
        public bool ApiAccountName_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.PayPalDirect.Fields.ApiAccountPassword")]
        public string ApiAccountPassword { get; set; }
        public bool ApiAccountPassword_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.PayPalDirect.Fields.Signature")]
        public string Signature { get; set; }
        public bool Signature_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.PayPalDirect.Fields.AdditionalFee")]
        public decimal AdditionalFee { get; set; }
        public bool AdditionalFee_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.PayPalDirect.Fields.AdditionalFeePercentage")]
        public bool AdditionalFeePercentage { get; set; }
        public bool AdditionalFeePercentage_OverrideForStore { get; set; }
    }
}