using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Misc.AbcCore.Areas.Admin.Models
{
    public partial record MissingImageProductSearchModel : BaseSearchModel
    {
        [NopResourceDisplayName("Admin.Catalog.Products.List.SearchProductName")]
        public string SearchProductName { get; set; }
    }
}