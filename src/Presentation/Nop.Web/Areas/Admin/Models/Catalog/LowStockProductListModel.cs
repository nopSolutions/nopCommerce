using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Mvc.Models;

namespace Nop.Web.Areas.Admin.Models.Catalog
{
    public class LowStockProductListModel : BaseNopModel
    {
        public LowStockProductListModel()
        {
            AvailablePublishedOptions = new List<SelectListItem>();
        }

        [NopResourceDisplayName("Admin.Catalog.LowStockReport.SearchPublished")]
        public int SearchPublishedId { get; set; }
        public IList<SelectListItem> AvailablePublishedOptions { get; set; }
    }
}