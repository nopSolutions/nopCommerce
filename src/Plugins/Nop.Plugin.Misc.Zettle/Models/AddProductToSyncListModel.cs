using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Framework.Models;

namespace Nop.Plugin.Misc.Zettle.Models;

/// <summary>
/// Represents a product list model to add for synchronization
/// </summary>
public record AddProductToSyncListModel : BasePagedListModel<ProductModel>
{
}