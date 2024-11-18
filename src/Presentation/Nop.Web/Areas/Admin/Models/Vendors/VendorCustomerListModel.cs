using Nop.Web.Areas.Admin.Models.Customers;
using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Vendors;

/// <summary>
/// Represents a vendor customer list model
/// </summary>
public partial record VendorCustomerListModel : BasePagedListModel<CustomerModel>
{
}
