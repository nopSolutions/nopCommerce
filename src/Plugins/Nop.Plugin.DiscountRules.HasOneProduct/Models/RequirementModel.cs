using Nop.Web.Framework;

namespace Nop.Plugin.DiscountRules.HasOneProduct.Models
{
    public class RequirementModel
    {
        [NopResourceDisplayName("Plugins.DiscountRules.HasOneProduct.Fields.Products")]
        public string Products { get; set; }

        public int DiscountId { get; set; }

        public int RequirementId { get; set; }
    }
}