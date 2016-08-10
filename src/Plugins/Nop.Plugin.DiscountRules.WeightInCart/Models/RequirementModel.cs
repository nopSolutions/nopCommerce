using Nop.Web.Framework;

namespace Nop.Plugin.DiscountRules.WeightInCart.Models
{
    public class RequirementModel
    {
        [NopResourceDisplayName("Plugins.DiscountRules.WeightInCart.Fields.WeightRange")]
        public string WeightRange { get; set; }

        public int DiscountId { get; set; }

        public int RequirementId { get; set; }

    }
}