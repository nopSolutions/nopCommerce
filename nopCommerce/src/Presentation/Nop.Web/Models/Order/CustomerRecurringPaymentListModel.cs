using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Order;

/// <summary>
/// Represents a customer recurring payment list model
/// </summary>
public partial record CustomerRecurringPaymentListModel : BaseNopModel
{
    #region Properties

    public List<CustomerRecurringPaymentModel> RecurringPayments { get; set; } = new();
    public List<string> RecurringPaymentErrors { get; set; } = new();

    #endregion
}