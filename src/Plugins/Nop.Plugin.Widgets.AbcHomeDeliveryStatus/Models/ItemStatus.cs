using System;
using System.Xml.Serialization;

namespace Nop.Plugin.Widgets.AbcHomeDeliveryStatus.Models
{
    // orderItem
    [Serializable()]
    public class ItemStatus
    {
        // ItemNumber (isam item # sigh)
        [XmlElement("ItemNumber")]
        public string Model { get; set; }

        // ItemBrand
        [XmlElement("ItemBrand")]
        public string Brand { get; set; }

        // ItemDescription
        [XmlElement("ItemDescription")]
        public string Name { get; set; }

        // DeliveryDate
        [XmlElement("DeliveryDate")]
        public string ScheduledDeliveryDate { get; set; }

        // OrdLnComment
        [XmlElement("OrdLnComment")]
        public string Comment { get; set; }
    }
}
