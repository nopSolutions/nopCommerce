using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Affiliates;

/// <summary>
/// Represents an affiliated customer list model
/// </summary>
public partial record AffiliatedCustomerListModel : BasePagedListModel<AffiliatedCustomerModel>
{
}