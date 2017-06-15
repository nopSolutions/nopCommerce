using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Mvc.Models;

namespace Nop.Plugin.Shipping.FixedOrByWeight.Models
{
    public class FixedRateModel : BaseNopModel
    {
        public int ShippingMethodId { get; set; }

        [NopResourceDisplayName("Plugins.Shipping.FixedOrByWeight.Fields.ShippingMethod")]
        public string ShippingMethodName { get; set; }

        [NopResourceDisplayName("Plugins.Shipping.FixedOrByWeight.Fields.Rate")]
        public decimal Rate { get; set; }
    }
}