using Nop.Web.Framework;

namespace Nop.Plugin.DiscountRules.PurchasedAllProducts.Models
{
    public class RequirementModel
    {
        [NopResourceDisplayName("Plugins.DiscountRules.PurchasedAllProducts.Fields.ProductVariants")]
        public string ProductVariants { get; set; }

        public int DiscountId { get; set; }

        public int RequirementId { get; set; }
    }
}