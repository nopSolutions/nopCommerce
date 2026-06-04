using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.PriceLists;

/// <summary>
/// Represents a price list item search model
/// </summary>
public partial record PriceListItemSearchModel : BaseSearchModel
{
    #region Properties

    public int PriceListId { get; set; }

    #endregion
}
