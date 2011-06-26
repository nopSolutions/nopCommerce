using System.ComponentModel;

namespace Nop.Plugin.Shipping.AustraliaPost.Models
{
    public class AustraliaPostShippingModel
    {
        [DisplayName("Gateway URL")]
        public string GatewayUrl { get; set; }

        [DisplayNameAttribute("Additional handling charge")]
        public decimal AdditionalHandlingCharge { get; set; }

        [DisplayNameAttribute("Shipped from zip")]
        public string ShippedFromZipPostalCode { get; set; }
    }
}