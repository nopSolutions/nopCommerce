using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents a product tag search model
    /// </summary>
    public partial record ProductTagSearchModel : BaseSearchModel
    {
        [NopResourceDisplayName("Admin.Catalog.ProductTags.Fields.SearchTagName")]
        public string SearchTagName { get; set; }
    }
}