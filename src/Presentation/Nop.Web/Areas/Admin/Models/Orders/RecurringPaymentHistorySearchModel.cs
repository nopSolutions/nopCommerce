using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Orders;

/// <summary>
/// Represents a recurring payment history search model
/// </summary>
public partial record RecurringPaymentHistorySearchModel : BaseSearchModel
{
    #region Properties

    public int RecurringPaymentId { get; set; }

    #endregion
}