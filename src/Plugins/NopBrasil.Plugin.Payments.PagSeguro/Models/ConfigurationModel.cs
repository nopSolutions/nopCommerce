using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace NopBrasil.Plugin.Payments.PagSeguro.Models
{
    public class ConfigurationModel : BaseNopModel
    {
        public int ActiveStoreScopeConfiguration { get; set; }

        [NopResourceDisplayName("Plugins.Payments.EmailAdmin.PagSeguro")]
        public string PagSeguroEmail { get; set; }

        public bool PagSeguroEmail_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.Token.PagSeguro")]
        public string PagSeguroToken { get; set; }

        public bool PagSeguroToken_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.MethodDescription.PagSeguro")]
        public string PaymentMethodDescription { get; set; }

        public bool PaymentMethodDescription_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.MethodDescription.PagSeguro.IsSandbox")]
        public bool IsSandbox { get; set; }

        public bool IsSandbox_OverrideForStore { get; set; }
    }
}
