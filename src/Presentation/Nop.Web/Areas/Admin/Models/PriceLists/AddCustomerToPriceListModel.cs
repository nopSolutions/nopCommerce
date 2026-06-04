using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.PriceLists;

/// <summary>
/// Represents a customer model to add to the price list
/// </summary>
public partial record AddCustomerToPriceListModel : BaseNopModel
{
    #region Ctor

    public AddCustomerToPriceListModel()
    {
        SelectedCustomerIds = new List<int>();
    }
    #endregion

    #region Properties

    public int PriceListId { get; set; }

    public IList<int> SelectedCustomerIds { get; set; }

    #endregion
}
