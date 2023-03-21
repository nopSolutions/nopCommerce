using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Payments.Param.Models
{
    public record ConfigurationModel : BaseNopModel
    {
        [NopResourceDisplayName("Plugins.Payments.Param.UseSandbox.Hint")]
        public bool UseSandbox { get; set; }

        [NopResourceDisplayName("Plugins.Payments.Param.ClientCode")]
        public string ClientCode { get; set; }

        [NopResourceDisplayName("Plugins.Payments.Param.ClientUsername")]
        public string ClientUsername { get; set; }

        [NopResourceDisplayName("Plugins.Payments.Param.ClientPassword")]
        public string ClientPassword { get; set; }

        [NopResourceDisplayName("Plugins.Payments.Param.Guid")]
        public string Guid { get; set; }

        [NopResourceDisplayName("Test Url:")]
        public string TestUrl { get; set; }

        [NopResourceDisplayName("Product Url:")]
        public string ProductUrl { get; set; }

        [NopResourceDisplayName("Plugins.Payments.Param.Installment.Hint")]
        public bool Installment { get; set; }

    }
}