using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Affiliates;

/// <summary>
/// Represents an affiliated order list model
/// </summary>
public partial record AffiliatedOrderListModel : BasePagedListModel<AffiliatedOrderModel>
{
}