using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.PriceLists;

/// <summary>
/// Represents a price list customer search model
/// </summary>
public partial record PriceListCustomerSearchModel : BaseSearchModel
{
    #region Properties

    public int PriceListId { get; set; }

    #endregion
}
