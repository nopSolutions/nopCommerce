using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Reports;

/// <summary>
/// Represents a bestseller model
/// </summary>
public partial record BestsellerModel : BaseNopModel
{
    #region Properties

    public int ProductId { get; set; }

    [NopResourceDisplayName("Admin.Reports.Sales.Bestsellers.Fields.Name")]
    public string ProductName { get; set; }

    [NopResourceDisplayName("Admin.Reports.Sales.Bestsellers.Fields.TotalAmount")]
    public string TotalAmount { get; set; }

    [NopResourceDisplayName("Admin.Reports.Sales.Bestsellers.Fields.TotalQuantity")]
    public decimal TotalQuantity { get; set; }

    #endregion
}