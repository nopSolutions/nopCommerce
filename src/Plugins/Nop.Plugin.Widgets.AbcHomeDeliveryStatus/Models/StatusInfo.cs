using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Nop.Plugin.Widgets.AbcHomeDeliveryStatus.Models
{
    [XmlRoot(ElementName = "homeDeliveryStatus")]
    public class StatusInfo
    {
        public string ErrorMessage { get; set; }

        [XmlElement("invoiceNumber")]
        public string InvoiceNumber { get; set; }

        // customerName
        [XmlElement("customerName")]
        public string CustomerName { get; set; }

        // shippingAddress
        [XmlElement("shippingAddress")]
        public string ShippingAddress { get; set; }

        // truckLoaded
        [XmlElement("truckLoaded")]
        public string TruckLoaded { get; set; }

        // deliveryTime
        [XmlElement("deliveryTime")]
        public string DeliveryTime { get; set; }

        // stopNumber
        [XmlElement("stopNumber")]
        public string StopNumber { get; set; }

        // storePhoneNumber
        [XmlElement("storePhoneNumber")]
        public string StorePhoneNumber { get; set; }

        // orderItems
        [XmlArray("orderItems")]
        [XmlArrayItem("orderItem", typeof(ItemStatus))]
        public ItemStatus[] ItemStatuses;
    }
}
