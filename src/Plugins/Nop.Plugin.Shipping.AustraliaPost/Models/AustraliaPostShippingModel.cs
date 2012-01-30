using System.ComponentModel;
using Nop.Web.Framework;

namespace Nop.Plugin.Shipping.AustraliaPost.Models
{
    public class AustraliaPostShippingModel
    {
        [NopResourceDisplayName("Plugins.Shipping.AustraliaPost.Fields.GatewayUrl")]
        public string GatewayUrl { get; set; }

        [DisplayName("Additional handling charge")]
        [NopResourceDisplayName("Plugins.Shipping.AustraliaPost.Fields.AdditionalHandlingCharge")]
        public decimal AdditionalHandlingCharge { get; set; }

        [DisplayNameAttribute("Shipped from zip")]
        [NopResourceDisplayName("Plugins.Shipping.AustraliaPost.Fields.ShippedFromZipPostalCode")]
        public string ShippedFromZipPostalCode { get; set; }
    }
}