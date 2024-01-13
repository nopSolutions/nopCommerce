using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Catalog;

/// <summary>
/// Represents a product attribute combination list model
/// </summary>
public partial record ProductAttributeCombinationListModel : BasePagedListModel<ProductAttributeCombinationModel>
{
}