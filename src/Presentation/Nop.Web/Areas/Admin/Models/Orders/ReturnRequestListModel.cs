using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Orders;

/// <summary>
/// Represents a return request list model
/// </summary>
public partial record ReturnRequestListModel : BasePagedListModel<ReturnRequestModel>
{
}