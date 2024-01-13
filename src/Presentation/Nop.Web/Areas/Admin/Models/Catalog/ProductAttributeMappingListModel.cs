using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Catalog;

/// <summary>
/// Represents a product attribute mapping list model
/// </summary>
public partial record ProductAttributeMappingListModel : BasePagedListModel<ProductAttributeMappingModel>
{
}