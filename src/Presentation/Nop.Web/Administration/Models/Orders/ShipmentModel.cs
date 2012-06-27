using System.Collections.Generic;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Models.Orders
{
    public partial class ShipmentModel : BaseNopEntityModel
    {
        public ShipmentModel()
        {
            this.Products = new List<ShipmentOrderProductVariantModel>();
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
        [NopResourceDisplayName("Admin.Orders.Shipments.DeliveryDate")]
        public string DeliveryDate { get; set; }
        public bool CanDeliver { get; set; }

        public List<ShipmentOrderProductVariantModel> Products { get; set; }

        public bool DisplayPdfPackagingSlip { get; set; }

        #region Nested classes

        public partial class ShipmentOrderProductVariantModel : BaseNopEntityModel
        {
            public int OrderProductVariantId { get; set; }
            public int ProductVariantId { get; set; }
            public string FullProductName { get; set; }
            public string Sku { get; set; }
            public string AttributeInfo { get; set; }
            
            //weight of one item (product variant)
            public string ItemWeight { get; set; }
            public string ItemDimensions { get; set; }

            public int QuantityToAdd { get; set; }
            public int QuantityOrdered { get; set; }
            public int QuantityInThisShipment { get; set; }
            public int QuantityInAllShipments { get; set; }
        }
        #endregion
    }
}