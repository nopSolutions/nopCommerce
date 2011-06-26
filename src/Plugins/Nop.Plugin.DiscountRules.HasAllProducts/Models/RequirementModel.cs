using Nop.Web.Framework;

namespace Nop.Plugin.DiscountRules.HasAllProducts.Models
{
    public class RequirementModel
    {
        [NopResourceDisplayName("Plugins.DiscountRules.HasAllProducts.Fields.ProductVariants")]
        public string ProductVariants { get; set; }

        public int DiscountId { get; set; }

        public int RequirementId { get; set; }
    }
}