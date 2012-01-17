using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.Payments.PurchaseOrder.Models
{
    public class ConfigurationModel : BaseNopModel
    {
        [NopResourceDisplayName("Plugins.Payment.PurchaseOrder.AdditionalFee")]
        public decimal AdditionalFee { get; set; }
    }
}