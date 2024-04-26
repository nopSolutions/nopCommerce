using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Discounts;

/// <summary>
/// Represents a discount list model
/// </summary>
public partial record DiscountListModel : BasePagedListModel<DiscountModel>
{
}