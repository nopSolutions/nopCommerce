using System.Collections.Generic;
using System.Web.Mvc;
using Nop.Web.Framework;

namespace Nop.Plugin.DiscountRules.Store.Models
{
    public class RequirementModel
    {
        public RequirementModel()
        {
            AvailableStores = new List<SelectListItem>();
        }

        [NopResourceDisplayName("Plugins.DiscountRules.Store.Fields.Store")]
        public int StoreId { get; set; }

        public int DiscountId { get; set; }

        public int RequirementId { get; set; }

        public IList<SelectListItem> AvailableStores { get; set; }
    }
}