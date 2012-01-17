using Nop.Web.Framework;

namespace Nop.Plugin.Shipping.FixedRateShipping.Models
{
    public class FixedShippingRateModel
    {
        public int ShippingMethodId { get; set; }

        [NopResourceDisplayName("Plugins.Shipping.FixedRateShipping.Fields.ShippingMethodName")]
        public string ShippingMethodName { get; set; }

        [NopResourceDisplayName("Plugins.Shipping.FixedRateShipping.Fields.Rate")]
        public decimal Rate { get; set; }
    }
}