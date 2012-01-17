using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.Payments.GoogleCheckout.Models
{
    public class ConfigurationModel : BaseNopModel
    {
        [NopResourceDisplayName("Plugins.Payments.GoogleCheckout.Fields.UseSandbox")]
        public bool UseSandbox { get; set; }

        [NopResourceDisplayName("Plugins.Payments.GoogleCheckout.Fields.GoogleVendorId")]
        public string GoogleVendorId { get; set; }

        [NopResourceDisplayName("Plugins.Payments.GoogleCheckout.Fields.GoogleMerchantKey")]
        public string GoogleMerchantKey { get; set; }

        [NopResourceDisplayName("Plugins.Payments.GoogleCheckout.Fields.AuthenticateCallback")]
        public bool AuthenticateCallback { get; set; }
    }
}