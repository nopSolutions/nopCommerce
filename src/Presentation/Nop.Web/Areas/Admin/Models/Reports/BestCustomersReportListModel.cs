using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Reports;

/// <summary>
/// Represents a best customers report list model
/// </summary>
public partial record BestCustomersReportListModel : BasePagedListModel<BestCustomersReportModel>
{
}