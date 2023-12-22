using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Catalog;

/// <summary>
/// Represents a review type list model
/// </summary>
public partial record ReviewTypeListModel : BasePagedListModel<ReviewTypeModel>
{
}