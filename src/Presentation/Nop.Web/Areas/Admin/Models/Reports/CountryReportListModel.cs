using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Reports;

/// <summary>
/// Represents a country report list model
/// </summary>
public partial record CountryReportListModel : BasePagedListModel<CountryReportModel>
{
}