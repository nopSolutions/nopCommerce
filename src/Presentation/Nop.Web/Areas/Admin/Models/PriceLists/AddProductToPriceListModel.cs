using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.PriceLists;

/// <summary>
/// Represents a product model to add to the price list
/// </summary>
public partial record AddProductToPriceListModel : BaseNopModel
{
    #region Ctor

    public AddProductToPriceListModel()
    {
        SelectedProductIds = new List<int>();
    }
    #endregion

    #region Properties

    public int PriceListId { get; set; }

    public IList<int> SelectedProductIds { get; set; }

    #endregion
}
