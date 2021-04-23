using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Orders
{
    /// <summary>
    /// Represents a shipment item list model
    /// </summary>
    public partial record ShipmentItemListModel : BasePagedListModel<ShipmentItemModel>
    {
    }
}