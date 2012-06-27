using System.Web.Mvc;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using Telerik.Web.Mvc;

namespace Nop.Admin.Models.Catalog
{
    public partial class CategoryListModel : BaseNopModel
    {
        [NopResourceDisplayName("Admin.Catalog.Categories.List.SearchCategoryName")]
        [AllowHtml]
        public string SearchCategoryName { get; set; }

        public GridModel<CategoryModel> Categories { get; set; }
    }
}