using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.PriceLists;

/// <summary>
/// Represents a price list item model
/// </summary>
public partial record PriceListItemModel : BaseNopEntityModel
{
    #region Properties

    public int PriceListId { get; set; }

    public int ProductId { get; set; }

    public string ProductName { get; set; }
    public string StandardPrice { get; set; }
    public string CalculatedPrice { get; set; }
    public decimal? ManualPrice { get; set; }

    #endregion
}