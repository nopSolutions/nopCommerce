using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Orders
{
    /// <summary>
    /// Represents a product list model to add to the order
    /// </summary>
    public partial record AddProductToOrderListModel : BasePagedListModel<ProductModel>
    {
    }
}