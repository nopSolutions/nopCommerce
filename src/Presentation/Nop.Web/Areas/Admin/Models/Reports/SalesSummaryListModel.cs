using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Reports;

/// <summary>
/// Represents a sales summary list model
/// </summary>
public partial record SalesSummaryListModel : BasePagedListModel<SalesSummaryModel>
{
}