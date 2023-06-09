using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.DiscountRules.CustomerRoles.Models
{
    public class RequirementModel
    {
        public RequirementModel()
        {
            AvailableCustomerRoles = new List<SelectListItem>();
        }

        [NopResourceDisplayName("Plugins.DiscountRules.CustomerRoles.Fields.CustomerRole")]
        public int CustomerRoleId { get; set; }

        public int DiscountId { get; set; }

        public int RequirementId { get; set; }

        public IList<SelectListItem> AvailableCustomerRoles { get; set; }
    }
}