using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Reports;

/// <summary>
/// Represents a never sold products report model
/// </summary>
public partial record NeverSoldReportModel : BaseNopModel
{
    #region Properties

    public int ProductId { get; set; }

    [NopResourceDisplayName("Admin.Reports.Sales.NeverSold.Fields.Name")]
    public string ProductName { get; set; }

    #endregion
}