using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Nop.Plugin.Widgets.AbcPickupInStore.Models
{
    [XmlRoot("Request")]
    public class StockRequest
    {
        [XmlElement(ElementName = "InventoryPickup")]
        public ItemNumber InventoryPickup { get; set; }

        [Serializable]
        public class ItemNumber
        {
            [XmlElement(ElementName = "ItemNumber")]
            public string BackendId { get; set; }
        }
    }
}
