using System;
using System.Collections.Generic;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Models.Orders
{
    public partial class ShipmentModel : BaseNopEntityModel
    {
        public ShipmentModel()
        {
            this.Items = new List<ShipmentItemModel>();
        }
        [NopResourceDisplayName("Admin.Orders.Shipments.ID")]
        public override int Id { get; set; }
        [NopResourceDisplayName("Admin.Orders.Shipments.OrderID")]
        public int OrderId { get; set; }
        [NopResourceDisplayName("Admin.Orders.Shipments.TotalWeight")]
        public string TotalWeight { get; set; }
        [NopResourceDisplayName("Admin.Orders.Shipments.TrackingNumber")]
        public string TrackingNumber { get; set; }

        [NopResourceDisplayName("Admin.Orders.Shipments.ShippedDate")]
        public string ShippedDate { get; set; }
        public bool CanShip { get; set; }
        public DateTime? ShippedDateUtc { get; set; }

        [NopResourceDisplayName("Admin.Orders.Shipments.DeliveryDate")]
        public string DeliveryDate { get; set; }
        public bool CanDeliver { get; set; }
        public DateTime? DeliveryDateUtc { get; set; }

        public List<ShipmentItemModel> Items { get; set; }

        #region Nested classes

        public partial class ShipmentItemModel : BaseNopEntityModel
        {
            public int OrderItemId { get; set; }
            public int ProductId { get; set; }
            [NopResourceDisplayName("Admin.Orders.Shipments.Products.ProductName")]
            public string ProductName { get; set; }
            public string Sku { get; set; }
            public string AttributeInfo { get; set; }

            //weight of one item (product)
            [NopResourceDisplayName("Admin.Orders.Shipments.Products.ItemWeight")]
            public string ItemWeight { get; set; }
            [NopResourceDisplayName("Admin.Orders.Shipments.Products.ItemDimensions")]
            public string ItemDimensions { get; set; }

            public int QuantityToAdd { get; set; }
            public int QuantityOrdered { get; set; }
            [NopResourceDisplayName("Admin.Orders.Shipments.Products.QtyShipped")]
            public int QuantityInThisShipment { get; set; }
            public int QuantityInAllShipments { get; set; }
        }
        #endregion
    }
}