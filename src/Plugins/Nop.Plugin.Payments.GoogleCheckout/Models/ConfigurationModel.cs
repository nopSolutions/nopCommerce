using System.ComponentModel;
using System.Web.Mvc;
using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.Payments.GoogleCheckout.Models
{
    public class ConfigurationModel : BaseNopModel
    {
        [DisplayName("Use Sandbox")]
        public bool UseSandbox { get; set; }

        [DisplayName("Google Vendor ID")]
        public string GoogleVendorId { get; set; }

        [DisplayName("Google Merchant Key")]
        public string GoogleMerchantKey { get; set; }

        [DisplayName("Authenticate callback")]
        public bool AuthenticateCallback { get; set; }
    }
}