using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents an associated product list model
    /// </summary>
    public partial record AssociatedProductListModel : BasePagedListModel<AssociatedProductModel>
    {
    }
}