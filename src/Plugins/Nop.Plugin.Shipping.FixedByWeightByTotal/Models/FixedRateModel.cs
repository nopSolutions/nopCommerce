using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Models;
using System.ComponentModel.DataAnnotations;

namespace Nop.Plugin.Shipping.FixedByWeightByTotal.Models
{
    public record FixedRateModel : BaseNopModel
    {
        public int ShippingMethodId { get; set; }

        [NopResourceDisplayName("Plugins.Shipping.FixedByWeightByTotal.Fields.ShippingMethod")]
        public string ShippingMethodName { get; set; }

        [NopResourceDisplayName("Plugins.Shipping.FixedByWeightByTotal.Fields.Rate")]
        public decimal Rate { get; set; }

        [UIHint("Int32Nullable")]
        [NopResourceDisplayName("Plugins.Shipping.FixedByWeightByTotal.Fields.TransitDays")]
        public int? TransitDays { get; set; }
    }
}