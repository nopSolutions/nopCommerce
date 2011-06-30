using System.Web.Mvc;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.Payments.CheckMoneyOrder.Models
{
    public class ConfigurationModel : BaseNopModel
    {
        [AllowHtml]
        [NopResourceDisplayName("Plugins.Payment.CheckMoneyOrder.DescriptionText")]
        public string DescriptionText { get; set; }

        [NopResourceDisplayName("Plugins.Payment.CheckMoneyOrder.AdditionalFee")]
        public decimal AdditionalFee { get; set; }
    }
}