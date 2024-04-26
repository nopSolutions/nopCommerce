using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Orders;

/// <summary>
/// Represents an order shipment list model
/// </summary>
public partial record OrderShipmentListModel : BasePagedListModel<ShipmentModel>
{
}