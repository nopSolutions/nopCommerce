using Nop.Web.Areas.Admin.Models.Customers;
using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.PriceLists;

/// <summary>
/// Represents a customer list model to add to the price list
/// </summary>
public partial record AddCustomerToPriceListListModel : BasePagedListModel<CustomerModel>
{
}
