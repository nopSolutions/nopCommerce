using System.Collections.Generic;
using System.ComponentModel;
using System.Web.Mvc;
using Nop.Plugin.Shipping.UPS.Domain;

namespace Nop.Plugin.Shipping.UPS.Models
{
    public class UPSShippingModel
    {
        public UPSShippingModel()
        {
            CarrierServicesOffered = new List<string>();
            AvailableCarrierServices = new List<string>();
            AvailableCustomerClassifications = new List<SelectListItem>();
            AvailablePickupTypes = new List<SelectListItem>();
            AvailablePackagingTypes = new List<SelectListItem>();
            AvailableCountries = new List<SelectListItem>();
        }
        [DisplayNameAttribute("URL")]
        public string Url { get; set; }

        [DisplayNameAttribute("Access Key")]
        public string AccessKey { get; set; }

        [DisplayNameAttribute("Username")]
        public string Username { get; set; }

        [DisplayNameAttribute("Password")]
        public string Password { get; set; }

        [DisplayNameAttribute("Additional handling charge")]
        public decimal AdditionalHandlingCharge { get; set; }

        [DisplayNameAttribute("UPS Customer Classification")]
        public string CustomerClassification { get; set; }
        public IList<SelectListItem> AvailableCustomerClassifications { get; set; }

        [DisplayNameAttribute("UPS Pickup Type")]
        public string PickupType { get; set; }
        public IList<SelectListItem> AvailablePickupTypes { get; set; }

        [DisplayNameAttribute("UPS Packaging Type")]
        public string PackagingType { get; set; }
        public IList<SelectListItem> AvailablePackagingTypes { get; set; }

        [DisplayNameAttribute("Shipped from country")]
        public int DefaultShippedFromCountryId { get; set; }
        public IList<SelectListItem> AvailableCountries { get; set; }

        [DisplayNameAttribute("Shipped from zip")]
        public string DefaultShippedFromZipPostalCode { get; set; }
        

        public IList<string> CarrierServicesOffered { get; set; }
        public IList<string> AvailableCarrierServices { get; set; }
        public string[] CheckedCarrierServices { get; set; }
    }
}