using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Framework.Models;

namespace Nop.Plugin.Tax.Avalara.Models.ItemClassification;

/// <summary>
/// Represents a product list model to add for classification
/// </summary>
public record AddProductToClassificationListModel : BasePagedListModel<ProductModel>
{
}