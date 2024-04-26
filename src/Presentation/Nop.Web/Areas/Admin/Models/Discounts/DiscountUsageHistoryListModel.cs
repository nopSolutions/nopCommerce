using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Discounts;

/// <summary>
/// Represents a discount usage history list model
/// </summary>
public partial record DiscountUsageHistoryListModel : BasePagedListModel<DiscountUsageHistoryModel>
{
}