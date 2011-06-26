using System.ComponentModel;

namespace Nop.Plugin.Shipping.FixedRateShipping.Models
{
    public class FixedShippingRateModel
    {
        public int ShippingMethodId { get; set; }

        [DisplayName("Shipping method")]
        public string ShippingMethodName { get; set; }

        [DisplayNameAttribute("Rate")]
        public decimal Rate { get; set; }
    }
}