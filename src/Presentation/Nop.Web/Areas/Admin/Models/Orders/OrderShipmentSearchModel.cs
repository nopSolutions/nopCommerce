using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Orders
{
    /// <summary>
    /// Represents an order shipment search model
    /// </summary>
    public partial class OrderShipmentSearchModel : BaseSearchModel
    {
        #region Ctor

        public OrderShipmentSearchModel()
        {
            ShipmentItemSearchModel = new ShipmentItemSearchModel();
        }

        #endregion

        #region Properties

        public int OrderId { get; set; }

        public ShipmentItemSearchModel ShipmentItemSearchModel { get; set; }

        #endregion
    }
}