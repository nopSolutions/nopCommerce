using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Orders;

/// <summary>
/// Represents an order list model
/// </summary>
public partial record OrderListModel : BasePagedListModel<OrderModel>
{
}