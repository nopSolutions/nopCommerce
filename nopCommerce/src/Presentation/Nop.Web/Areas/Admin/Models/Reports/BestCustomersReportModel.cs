using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Reports;

/// <summary>
/// Represents a best customers report model
/// </summary>
public partial record BestCustomersReportModel : BaseNopModel
{
    #region Properties

    public int CustomerId { get; set; }

    [NopResourceDisplayName("Admin.Reports.Customers.BestBy.Fields.Customer")]
    public string CustomerName { get; set; }

    [NopResourceDisplayName("Admin.Reports.Customers.BestBy.Fields.OrderTotal")]
    public string OrderTotal { get; set; }

    [NopResourceDisplayName("Admin.Reports.Customers.BestBy.Fields.OrderCount")]
    public decimal OrderCount { get; set; }

    #endregion
}