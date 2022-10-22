using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.PolyCommerce.Models.GetOrders
{
    public class PolyCommerceShipmentHeader
    {
        public int ShipmentId { get; set; }
        public int OrderId { get; set; }
        public int ShippingStatusId { get; set; }
        public DateTime ShippedDate { get; set; }
        public string ShippingMethod { get; set; }
        public string TrackingNumber { get; set; }
        public List<PolyCommerceOrderItem> OrderItems { get; set; }
    }
}
