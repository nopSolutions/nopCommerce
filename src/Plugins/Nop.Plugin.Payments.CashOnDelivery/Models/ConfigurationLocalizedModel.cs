using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Payments.CashOnDelivery.Models
{
    public record ConfigurationLocalizedModel : ILocalizedLocaleModel
    {
        [NopResourceDisplayName("Plugins.Payment.CashOnDelivery.DescriptionText")]
        public string DescriptionText { get; set; }

        public int LanguageId { get; set; }
    }
}
