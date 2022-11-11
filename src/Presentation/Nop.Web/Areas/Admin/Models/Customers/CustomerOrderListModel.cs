using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Customers
{
    /// <summary>
    /// Represents a customer order list model
    /// </summary>
    public partial record CustomerOrderListModel : BasePagedListModel<CustomerOrderModel>
    {
    }
}