using Nop.Web.Areas.Admin.Models.Orders;
using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents a product order list model
    /// </summary>
    public partial record ProductOrderListModel : BasePagedListModel<OrderModel>
    {
    }
}