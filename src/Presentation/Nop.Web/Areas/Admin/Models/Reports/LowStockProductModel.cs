using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Reports;

/// <summary>
/// Represents a low stock product model
/// </summary>
public partial record LowStockProductModel : BaseNopEntityModel
{
    #region Properties

    [NopResourceDisplayName("Admin.Catalog.Products.Fields.Name")]
    public string Name { get; set; }

    public string Attributes { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Products.Fields.ManageInventoryMethod")]
    public string ManageInventoryMethod { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Products.Fields.StockQuantity")]
    public int StockQuantity { get; set; }

    [NopResourceDisplayName("Admin.Catalog.Products.Fields.Published")]
    public bool Published { get; set; }

    #endregion
}