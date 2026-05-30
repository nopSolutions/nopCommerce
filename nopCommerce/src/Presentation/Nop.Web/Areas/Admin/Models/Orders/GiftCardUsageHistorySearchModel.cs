using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Orders;

/// <summary>
/// Represents a gift card usage history search model
/// </summary>
public partial record GiftCardUsageHistorySearchModel : BaseSearchModel
{
    #region Properties

    public int GiftCardId { get; set; }

    #endregion
}