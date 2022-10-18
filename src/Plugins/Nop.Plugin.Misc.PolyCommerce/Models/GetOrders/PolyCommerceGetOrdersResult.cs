using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.PolyCommerce.Models.GetOrders
{
    public class PolyCommerceGetOrdersResult
    {
        public List<PolyCommerceShipmentHeader> Shipments { get; set; }
    }
}
