using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Payments.MercadoPago.Models
{
    public class ConfigurationModel : BaseNopModel
    {
        public int ActiveStoreScopeConfiguration { get; set; }

        [NopResourceDisplayName("Plugins.Payments.MercadoPago.Fields.PublicKey")]
        public string PublicKey { get; set; }
        public bool PublicKey_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.MercadoPago.Fields.AccessToken")]
        public string AccessToken { get; set; }
        public bool AccessToken_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.MercadoPago.Fields.PublicKeySandbox")]
        public string PublicKeySandbox { get; set; }
        public bool PublicKeySandbox_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.MercadoPago.Fields.AccessTokenSandbox")]
        public string AccessTokenSandbox { get; set; }
        public bool AccessTokenSandbox_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.MercadoPago.Fields.UseSandbox")]
        public bool UseSandbox { get; set; }
        public bool UseSandbox_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.MercadoPago.Fields.PassPurchasedItems")]
        public bool PassPurchasedItems { get; set; }
        public bool PassPurchasedItems_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.MercadoPago.Fields.MethodDescription")]
        public string PaymentMethodDescription { get; set; }
        public bool PaymentMethodDescription_OverrideForStore { get; set; }
    }
}