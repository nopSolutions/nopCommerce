using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Discounts;

/// <summary>
/// Represents a discount category list model
/// </summary>
public partial record DiscountCategoryListModel : BasePagedListModel<DiscountCategoryModel>
{
}