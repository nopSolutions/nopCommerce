using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Nop.Plugin.Misc.AbcCore.Models
{
    [XmlRoot(ElementName = "Response")]
    public class StockResponse
    {
        [XmlArray("InventoryPickup")]
        [XmlArrayItem("BranchItem", typeof(ProductStock))]
        public List<ProductStock> ProductStocks;

        public int ProductId;

        public bool IsPickupOnlyMode;
    }
}
