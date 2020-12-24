using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Orders
{
    /// <summary>
    /// Represents a shipment list model
    /// </summary>
    public partial record ShipmentListModel : BasePagedListModel<ShipmentModel>
    {
    }
}