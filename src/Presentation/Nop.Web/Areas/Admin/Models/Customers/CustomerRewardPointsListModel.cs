using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Customers;

/// <summary>
/// Represents a reward points list model
/// </summary>
public partial record CustomerRewardPointsListModel : BasePagedListModel<CustomerRewardPointsModel>
{
}