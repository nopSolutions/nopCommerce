using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.PriceLists;

/// <summary>
/// Represents a product list model to add to the price list
/// </summary>
public partial record AddProductToPriceListListModel : BasePagedListModel<ProductModel>
{
}
