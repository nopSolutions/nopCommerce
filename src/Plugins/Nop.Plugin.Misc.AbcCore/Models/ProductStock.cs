using SevenSpikes.Nop.Plugins.StoreLocator.Domain.Shops;
using System;
using System.Xml.Serialization;

namespace Nop.Plugin.Misc.AbcCore.Models
{
    [Serializable]
    public class ProductStock
    {
        [XmlElement("BranchID")]
        public string BranchId;

        [XmlElement("Message")]
        public string Message;

        [XmlElement("Available")]
        public bool Available;

        [XmlElement("Quantity")]
        public int Quantity;

        public Shop Shop;

        public string ShopUrl;
    }
}
