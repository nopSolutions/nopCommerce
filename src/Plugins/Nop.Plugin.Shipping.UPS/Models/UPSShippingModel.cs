using System.Collections.Generic;
using System.Web.Mvc;
using Nop.Web.Framework;

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
        }
        [NopResourceDisplayName("Plugins.Shipping.UPS.Fields.Url")]
        public string Url { get; set; }

        [NopResourceDisplayName("Plugins.Shipping.UPS.Fields.AccessKey")]
        public string AccessKey { get; set; }

        [NopResourceDisplayName("Plugins.Shipping.UPS.Fields.Username")]
        public string Username { get; set; }

        [NopResourceDisplayName("Plugins.Shipping.UPS.Fields.Password")]
        public string Password { get; set; }

        [NopResourceDisplayName("Plugins.Shipping.UPS.Fields.AdditionalHandlingCharge")]
        public decimal AdditionalHandlingCharge { get; set; }

        [NopResourceDisplayName("Plugins.Shipping.UPS.Fields.InsurePackage")]
        public bool InsurePackage { get; set; }

        [NopResourceDisplayName("Plugins.Shipping.UPS.Fields.CustomerClassification")]
        public string CustomerClassification { get; set; }
        public IList<SelectListItem> AvailableCustomerClassifications { get; set; }

        [NopResourceDisplayName("Plugins.Shipping.UPS.Fields.PickupType")]
        public string PickupType { get; set; }
        public IList<SelectListItem> AvailablePickupTypes { get; set; }

        [NopResourceDisplayName("Plugins.Shipping.UPS.Fields.PackagingType")]
        public string PackagingType { get; set; }
        public IList<SelectListItem> AvailablePackagingTypes { get; set; }
        
        public IList<string> CarrierServicesOffered { get; set; }
        [NopResourceDisplayName("Plugins.Shipping.UPS.Fields.AvailableCarrierServices")]
        public IList<string> AvailableCarrierServices { get; set; }
        public string[] CheckedCarrierServices { get; set; }

        [NopResourceDisplayName("Plugins.Shipping.UPS.Fields.PassDimensions")]
        public bool PassDimensions { get; set; }

        [NopResourceDisplayName("Plugins.Shipping.UPS.Fields.PackingPackageVolume")]
        public int PackingPackageVolume { get; set; }

        public int PackingType { get; set; }
        [NopResourceDisplayName("Plugins.Shipping.UPS.Fields.PackingType")]
        public SelectList PackingTypeValues { get; set; }

        [NopResourceDisplayName("Plugins.Shipping.UPS.Fields.Tracing")]
        public bool Tracing { get; set; }
    }
}