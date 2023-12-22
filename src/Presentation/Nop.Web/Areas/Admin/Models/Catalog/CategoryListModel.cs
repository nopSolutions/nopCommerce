using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Catalog;

/// <summary>
/// Represents a category list model
/// </summary>
public partial record CategoryListModel : BasePagedListModel<CategoryModel>
{
}