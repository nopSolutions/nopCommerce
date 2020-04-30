using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Plugin.Misc.PolyCommerce.Models
{
    public class PolyCommerceCheckForShippedOrdersResult
    {
        public int OrderId { get; set; }
        public bool Shipped { get; set; }
        public string TrackingNumber { get; set; }
    }
}
