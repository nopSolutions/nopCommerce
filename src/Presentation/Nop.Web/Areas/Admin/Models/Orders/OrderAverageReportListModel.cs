using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Orders;

/// <summary>
/// Represents an an order average report line summary list model
/// </summary>
public partial record OrderAverageReportListModel : BasePagedListModel<OrderAverageReportModel>
{
}