using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Shipping.UPS.Models
{
    public record UPSShippingModel : BaseNopModel
    {
        #region Ctor

        public UPSShippingModel()
        {
            CarrierServices = new List<string>();
            AvailableCarrierServices = new List<SelectListItem>();
            AvailableCustomerClassifications = new List<SelectListItem>();
            AvailablePickupTypes = new List<SelectListItem>();
            AvailablePackagingTypes = new List<SelectListItem>();
            AvailablePackingTypes = new List<SelectListItem>();
            AvailableWeightTypes = new List<SelectListItem>();
            AvailableDimensionsTypes = new List<SelectListItem>();
        }

        #endregion

        #region Properties

        [NopResourceDisplayName("Plugins.Shipping.UPS.Fields.AccountNumber")]
        public string AccountNumber { get; set; }

        [NopResourceDisplayName("Plugins.Shipping.UPS.Fields.AccessKey")]
        public string AccessKey { get; set; }

        [NopResourceDisplayName("Plugins.Shipping.UPS.Fields.Username")]
        public string Username { get; set; }

        [NopResourceDisplayName("Plugins.Shipping.UPS.Fields.Password")]
        public string Password { get; set; }

        [NopResourceDisplayName("Plugins.Shipping.UPS.Fields.UseSandbox")]
        public bool UseSandbox { get; set; }

        [NopResourceDisplayName("Plugins.Shipping.UPS.Fields.AdditionalHandlingCharge")]
        public decimal AdditionalHandlingCharge { get; set; }

        [NopResourceDisplayName("Plugins.Shipping.UPS.Fields.InsurePackage")]
        public bool InsurePackage { get; set; }

        [NopResourceDisplayName("Plugins.Shipping.UPS.Fields.CustomerClassification")]
        public int CustomerClassification { get; set; }
        public IList<SelectListItem> AvailableCustomerClassifications { get; set; }

        [NopResourceDisplayName("Plugins.Shipping.UPS.Fields.PickupType")]
        public int PickupType { get; set; }
        public IList<SelectListItem> AvailablePickupTypes { get; set; }

        [NopResourceDisplayName("Plugins.Shipping.UPS.Fields.PackagingType")]
        public int PackagingType { get; set; }
        public IList<SelectListItem> AvailablePackagingTypes { get; set; }

        [NopResourceDisplayName("Plugins.Shipping.UPS.Fields.AvailableCarrierServices")]
        public IList<SelectListItem> AvailableCarrierServices { get; set; }
        public IList<string> CarrierServices { get; set; }

        [NopResourceDisplayName("Plugins.Shipping.UPS.Fields.SaturdayDeliveryEnabled")]
        public bool SaturdayDeliveryEnabled { get; set; }

        [NopResourceDisplayName("Plugins.Shipping.UPS.Fields.PassDimensions")]
        public bool PassDimensions { get; set; }

        [NopResourceDisplayName("Plugins.Shipping.UPS.Fields.PackingPackageVolume")]
        public int PackingPackageVolume { get; set; }

        [NopResourceDisplayName("Plugins.Shipping.UPS.Fields.PackingType")]
        public int PackingType { get; set; }
        public IList<SelectListItem> AvailablePackingTypes { get; set; }

        [NopResourceDisplayName("Plugins.Shipping.UPS.Fields.Tracing")]
        public bool Tracing { get; set; }

        [NopResourceDisplayName("Plugins.Shipping.UPS.Fields.WeightType")]
        public string WeightType { get; set; }
        public IList<SelectListItem> AvailableWeightTypes { get; set; }

        [NopResourceDisplayName("Plugins.Shipping.UPS.Fields.DimensionsType")]
        public string DimensionsType { get; set; }
        public IList<SelectListItem> AvailableDimensionsTypes { get; set; }

        #endregion
    }
}