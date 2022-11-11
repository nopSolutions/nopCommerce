using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents a product list model
    /// </summary>
    public partial record ProductListModel : BasePagedListModel<ProductModel>
    {
    }
}