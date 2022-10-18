using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Nop.Plugin.Misc.PolyCommerce.Models.GetOrders
{
    public class PolyCommerceOrderItem
    {
        public DateTime ShippedDate { get; set; }
        public string TrackingNumber { get; set; }

        [JsonIgnore]
        public int OrderId { get; set; }
        public int ShippedQuantity { get; set; }
        public int OrderedQuantity { get; set; }
        public int ProductId { get; set; }

        [JsonIgnore]
        public int ShipmentId { get; set; }

        [JsonIgnore]
        public int ShippingStatusId { get; set; }
        public string ShippingMethod { get; set; }

    }
}
