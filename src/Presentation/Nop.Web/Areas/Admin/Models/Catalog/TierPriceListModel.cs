using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Catalog;

/// <summary>
/// Represents a tier price list model
/// </summary>
public partial record TierPriceListModel : BasePagedListModel<TierPriceModel>
{
}