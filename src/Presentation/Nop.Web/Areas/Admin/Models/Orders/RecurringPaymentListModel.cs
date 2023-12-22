using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Orders;

/// <summary>
/// Represents a recurring payment list model
/// </summary>
public partial record RecurringPaymentListModel : BasePagedListModel<RecurringPaymentModel>
{
}