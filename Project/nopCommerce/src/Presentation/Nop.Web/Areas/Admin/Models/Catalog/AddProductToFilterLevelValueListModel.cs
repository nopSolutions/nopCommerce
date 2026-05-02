using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Catalog;

/// <summary>
/// Represents a product list model to add to the filter level value
/// </summary>
public partial record AddProductToFilterLevelValueListModel : BasePagedListModel<ProductModel>
{
}
