using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.PriceLists;

/// <summary>
/// Represents a price list item list model
/// </summary>
public partial record PriceListItemListModel : BasePagedListModel<PriceListItemModel>
{
}
