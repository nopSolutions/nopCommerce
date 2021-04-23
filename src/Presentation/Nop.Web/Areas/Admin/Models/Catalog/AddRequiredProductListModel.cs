using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents a required product list model to add to the product
    /// </summary>
    public partial record AddRequiredProductListModel : BasePagedListModel<ProductModel>
    {
    }
}