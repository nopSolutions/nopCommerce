using Nop.Web.Areas.Admin.Models.Customers;
using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Affiliates;

/// <summary>
/// Represents an affiliate  customer list model
/// </summary>
public partial record AffiliateCustomerListModel : BasePagedListModel<CustomerModel>;