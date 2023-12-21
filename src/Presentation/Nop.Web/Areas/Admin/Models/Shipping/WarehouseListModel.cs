using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Shipping;

/// <summary>
/// Represents a warehouse list model
/// </summary>
public partial record WarehouseListModel : BasePagedListModel<WarehouseModel>
{
}