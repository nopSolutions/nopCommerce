using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Reports;

/// <summary>
/// Represents a sales summary model
/// </summary>
public partial record SalesSummaryModel : BaseNopModel
{
    #region Properties

    [NopResourceDisplayName("Admin.Reports.SalesSummary.Fields.Summary")]
    public string Summary { get; set; }

    [NopResourceDisplayName("Admin.Reports.SalesSummary.Fields.NumberOfOrders")]
    public int NumberOfOrders { get; set; }

    [NopResourceDisplayName("Admin.Reports.SalesSummary.Fields.Profit")]
    public string ProfitStr { get; set; }

    [NopResourceDisplayName("Admin.Reports.SalesSummary.Fields.Shipping")]
    public string Shipping { get; set; }

    [NopResourceDisplayName("Admin.Reports.SalesSummary.Fields.Tax")]
    public string Tax { get; set; }

    [NopResourceDisplayName("Admin.Reports.SalesSummary.Fields.OrderTotal")]
    public string OrderTotal { get; set; }

    #endregion
}