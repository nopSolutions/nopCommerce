using System.Collections.Generic;
using System.ComponentModel;

namespace Nop.Plugin.Shipping.USPS.Models
{
    public class USPSShippingModel
    {
        public USPSShippingModel()
        {
            CarrierServicesOfferedDomestic = new List<string>();
            AvailableCarrierServicesDomestic = new List<string>();
            CarrierServicesOfferedInternational = new List<string>();
            AvailableCarrierServicesInternational = new List<string>();
        }

        [DisplayName("URL")]
        public string Url { get; set; }
        
        [DisplayName("Username")]
        public string Username { get; set; }
        
        [DisplayName("Password")]
        public string Password { get; set; }

        [DisplayNameAttribute("Additional handling charge")]
        public decimal AdditionalHandlingCharge { get; set; }
        
        [DisplayName("Shipped from zip")]
        public string ZipPostalCodeFrom { get; set; }

        public IList<string> CarrierServicesOfferedDomestic { get; set; }
        public IList<string> AvailableCarrierServicesDomestic { get; set; }
        public string[] CheckedCarrierServicesDomestic { get; set; }

        public IList<string> CarrierServicesOfferedInternational { get; set; }
        public IList<string> AvailableCarrierServicesInternational { get; set; }
        public string[] CheckedCarrierServicesInternational { get; set; }
    }
}