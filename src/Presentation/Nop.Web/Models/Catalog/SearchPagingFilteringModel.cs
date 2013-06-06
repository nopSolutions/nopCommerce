using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.UI.Paging;

namespace Nop.Web.Models.Catalog
{
    public partial class SearchPagingFilteringModel : BasePageableModel
    {
        public class CategoryModel : BaseNopEntityModel
        {
            public string Breadcrumb { get; set; }
        }
    }
}