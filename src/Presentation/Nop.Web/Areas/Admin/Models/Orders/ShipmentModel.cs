using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Orders
{
    /// <summary>
    /// Represents a shipment model
    /// </summary>
    public partial record ShipmentModel : BaseNopEntityModel
    {
        #region Ctor

        public ShipmentModel()
        {
            ShipmentStatusEvents = new List<ShipmentStatusEventModel>();
            Items = new List<ShipmentItemModel>();
        }

        #endregion

        #region Properties

        [NopResourceDisplayName("Admin.Orders.Shipments.ID")]
        public override int Id { get; set; }

        public int OrderId { get; set; }

        [NopResourceDisplayName("Admin.Orders.Shipments.PickupInStore")]
        public bool PickupInStore { get; set; }

        [NopResourceDisplayName("Admin.Orders.Shipments.CustomOrderNumber")]
        public string CustomOrderNumber { get; set; }

        [NopResourceDisplayName("Admin.Orders.Shipments.TotalWeight")]
        public string TotalWeight { get; set; }

        [NopResourceDisplayName("Admin.Orders.Shipments.TrackingNumber")]
        public string TrackingNumber { get; set; }

        public string TrackingNumberUrl { get; set; }

        [NopResourceDisplayName("Admin.Orders.Shipments.ShippedDate")]
        public string ShippedDate { get; set; }

        [NopResourceDisplayName("Admin.Orders.Shipments.CanShip")]
        public bool CanShip { get; set; }

        public DateTime? ShippedDateUtc { get; set; }

        [NopResourceDisplayName("Admin.Orders.Shipments.ReadyForPickupDate")]
        public string ReadyForPickupDate { get; set; }

        [NopResourceDisplayName("Admin.Orders.Shipments.CanMarkAsReadyForPickup")]
        public bool CanMarkAsReadyForPickup { get; set; }

        public DateTime? ReadyForPickupDateUtc { get; set; }

        [NopResourceDisplayName("Admin.Orders.Shipments.DeliveryDate")]
        public string DeliveryDate { get; set; }

        [NopResourceDisplayName("Admin.Orders.Shipments.CanDeliver")]
        public bool CanDeliver { get; set; }

        public DateTime? DeliveryDateUtc { get; set; }

        [NopResourceDisplayName("Admin.Orders.Shipments.AdminComment")]
        public string AdminComment { get; set; }

        public List<ShipmentItemModel> Items { get; set; }

        public IList<ShipmentStatusEventModel> ShipmentStatusEvents { get; set; }

        #endregion
    }
}