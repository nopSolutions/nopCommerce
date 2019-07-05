using Nop.Web.Areas.Admin.Models.Common;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Settings
{
    /// <summary>
    /// Represents a shipping settings model
    /// </summary>
    public partial class ShippingSettingsModel : BaseNopModel, ISettingsModel
    {
        #region Ctor

        public ShippingSettingsModel()
        {
            ShippingOriginAddress = new AddressModel();
        }

        #endregion

        #region Properties

        public int ActiveStoreScopeConfiguration { get; set; }
        
        [NopResourceDisplayName("Admin.Configuration.Settings.Shipping.ShipToSameAddress")]
        public bool ShipToSameAddress { get; set; }
        public bool ShipToSameAddress_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Shipping.AllowPickupInStore")]
        public bool AllowPickupInStore { get; set; }
        public bool AllowPickupInStore_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Shipping.DisplayPickupPointsOnMap")]
        public bool DisplayPickupPointsOnMap { get; set; }
        public bool DisplayPickupPointsOnMap_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Shipping.IgnoreAdditionalShippingChargeForPickupInStore")]
        public bool IgnoreAdditionalShippingChargeForPickupInStore { get; set; }
        public bool IgnoreAdditionalShippingChargeForPickupInStore_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Shipping.GoogleMapsApiKey")]
        public string GoogleMapsApiKey { get; set; }
        public bool GoogleMapsApiKey_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Shipping.UseWarehouseLocation")]
        public bool UseWarehouseLocation { get; set; }
        public bool UseWarehouseLocation_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Shipping.NotifyCustomerAboutShippingFromMultipleLocations")]
        public bool NotifyCustomerAboutShippingFromMultipleLocations { get; set; }
        public bool NotifyCustomerAboutShippingFromMultipleLocations_OverrideForStore { get; set; }
        
        [NopResourceDisplayName("Admin.Configuration.Settings.Shipping.FreeShippingOverXEnabled")]
        public bool FreeShippingOverXEnabled { get; set; }
        public bool FreeShippingOverXEnabled_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Shipping.FreeShippingOverXValue")]
        public decimal FreeShippingOverXValue { get; set; }
        public bool FreeShippingOverXValue_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Shipping.FreeShippingOverXIncludingTax")]
        public bool FreeShippingOverXIncludingTax { get; set; }
        public bool FreeShippingOverXIncludingTax_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Shipping.EstimateShippingEnabled")]
        public bool EstimateShippingEnabled { get; set; }
        public bool EstimateShippingEnabled_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Shipping.DisplayShipmentEventsToCustomers")]
        public bool DisplayShipmentEventsToCustomers { get; set; }
        public bool DisplayShipmentEventsToCustomers_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Shipping.DisplayShipmentEventsToStoreOwner")]
        public bool DisplayShipmentEventsToStoreOwner { get; set; }
        public bool DisplayShipmentEventsToStoreOwner_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Shipping.HideShippingTotal")]
        public bool HideShippingTotal { get; set; }
        public bool HideShippingTotal_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Shipping.BypassShippingMethodSelectionIfOnlyOne")]
        public bool BypassShippingMethodSelectionIfOnlyOne { get; set; }
        public bool BypassShippingMethodSelectionIfOnlyOne_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Shipping.ConsiderAssociatedProductsDimensions")]
        public bool ConsiderAssociatedProductsDimensions { get; set; }
        public bool ConsiderAssociatedProductsDimensions_OverrideForStore { get; set; }

        public AddressModel ShippingOriginAddress { get; set; }
        public bool ShippingOriginAddress_OverrideForStore { get; set; }

        public string PrimaryStoreCurrencyCode { get; set; }

        #endregion
    }
}