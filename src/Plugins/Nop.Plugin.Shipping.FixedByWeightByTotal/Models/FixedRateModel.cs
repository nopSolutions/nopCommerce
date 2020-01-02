using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Models;

namespace Nop.Plugin.Shipping.FixedByWeightByTotal.Models
{
    public class FixedRateModel : BaseNopModel
    {
        public int ShippingMethodId { get; set; }

        [NopResourceDisplayName("Plugins.Shipping.FixedByWeightByTotal.Fields.ShippingMethod")]
        public string ShippingMethodName { get; set; }

        [NopResourceDisplayName("Plugins.Shipping.FixedByWeightByTotal.Fields.Rate")]
        public decimal Rate { get; set; }
    }
}