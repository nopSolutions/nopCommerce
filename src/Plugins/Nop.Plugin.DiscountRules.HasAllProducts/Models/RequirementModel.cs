using Nop.Web.Framework;

namespace Nop.Plugin.DiscountRules.HasAllProducts.Models
{
    public class RequirementModel
    {
        [NopResourceDisplayName("Plugins.DiscountRules.HasAllProducts.Fields.Products")]
        public string Products { get; set; }

        public int DiscountId { get; set; }

        public int RequirementId { get; set; }
    }
}