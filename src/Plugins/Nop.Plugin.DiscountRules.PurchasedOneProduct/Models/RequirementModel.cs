using Nop.Web.Framework;

namespace Nop.Plugin.DiscountRules.PurchasedOneProduct.Models
{
    public class RequirementModel
    {
        [NopResourceDisplayName("Plugins.DiscountRules.PurchasedOneProduct.Fields.ProductVariants")]
        public string ProductVariants { get; set; }

        public int DiscountId { get; set; }

        public int RequirementId { get; set; }
    }
}