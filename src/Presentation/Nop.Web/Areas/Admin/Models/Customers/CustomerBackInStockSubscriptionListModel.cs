using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Customers;

/// <summary>
/// Represents a customer back in stock subscriptions list model
/// </summary>
public partial record CustomerBackInStockSubscriptionListModel : BasePagedListModel<CustomerBackInStockSubscriptionModel>
{
}