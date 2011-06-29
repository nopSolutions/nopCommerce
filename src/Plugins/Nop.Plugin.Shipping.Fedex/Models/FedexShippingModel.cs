using System.Collections.Generic;
using System.ComponentModel;

namespace Nop.Plugin.Shipping.Fedex.Models
{
    public class FedexShippingModel
    {
        public FedexShippingModel()
        {
            CarrierServicesOffered = new List<string>();
            AvailableCarrierServices = new List<string>();
        }
        [DisplayName("URL")]
        public string Url { get; set; }

        [DisplayName("Key")]
        public string Key { get; set; }

        [DisplayName("Password")]
        public string Password { get; set; }

        [DisplayName("Account number")]
        public string AccountNumber { get; set; }

        [DisplayName("Meter number")]
        public string MeterNumber { get; set; }

        [DisplayName("Use residential rates")]
        public bool UseResidentialRates { get; set; }

        [DisplayName("Use discounted Rates (instead of list rates):")]
        public bool ApplyDiscounts { get; set; }

        [DisplayNameAttribute("Additional handling charge")]
        public decimal AdditionalHandlingCharge { get; set; }
        
        public IList<string> CarrierServicesOffered { get; set; }
        public IList<string> AvailableCarrierServices { get; set; }
        public string[] CheckedCarrierServices { get; set; }

        [DisplayName("Shipping origin. Street")]
        public string Street { get; set; }

        [DisplayName("Shipping origin. City")]
        public string City { get; set; }

        [DisplayName("Shipping origin. State code (2 characters)")]
        public string StateOrProvinceCode { get; set; }

        [DisplayName("Shipping origin. Zip")]
        public string PostalCode { get; set; }

        [DisplayName("Shipping origin. Country code")]
        public string CountryCode { get; set; }
    }
}