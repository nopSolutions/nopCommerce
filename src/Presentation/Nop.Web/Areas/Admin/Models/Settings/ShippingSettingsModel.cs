using Nop.Web.Areas.Admin.Models.Common;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Settings
{
    /// <summary>
    ///     Represents a shipping settings model
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

        urceDisplayName("Admin.Configuration.Settings.Shipping.ShipToSameAddress")]
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

        [NopResourceDisplayName("Admin.C
            onfiguration.Settings.Shipping.NotifyCustomerAboutShippingFromMultipleLocations")]
        public bool NotifyCustomerAboutShippingFromMultipleLocations { get; set; }
        
public bool NotifyCustomerAboutShippingFromMultipleLocations_OverrideForStore { get; set; }

    

        layName("Admin.Configuration.Settings.Shipping.FreeShippingOverXEnabled")]
        public bool FreeShippingOverXEnabled { get; set; }
        public b
ool FreeShippingOverXEnabled_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Shipping.FreeShippingOverXValue")]
        public decimal FreeShippingOverXValue { get; set; }
        public b
ool FreeShippingOverXValue_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Shipping.FreeShippingOverXIncludingTax")]
        public bool FreeShippingOverXIncludingTax { get; set; }
        public b
ool FreeShippingOverXIncludingTax_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Shipping.EstimateShippingCartPageEnabled")]
        public bool EstimateShippingCartPageEnabled { get; set; }
        public b
ool EstimateShippingCartPageEnabled_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Shipping.EstimateShippingProductPageEnabled")]
        public bool EstimateShippingProductPageEnabled { get; set; }
        public b
ool EstimateShippingProductPageEnabled_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Shipping.DisplayShipmentEventsToCustomers")]
        public bool DisplayShipmentEventsToCustomers { get; set; }
        public b
ool DisplayShipmentEventsToCustomers_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Shipping.DisplayShipmentEventsToStoreOwner")]
        public bool DisplayShipmentEventsToStoreOwner { get; set; }
        public b
ool DisplayShipmentEventsToStoreOwner_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Shipping.HideShippingTotal")]
        public bool HideShippingTotal { get; set; }
        public b
ool HideShippingTotal_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Shipping.BypassShippingMethodSelectionIfOnlyOne")]
        public bool BypassShippingMethodSelectionIfOnlyOne { get; set; }
        public b
ool BypassShippingMethodSelectionIfOnlyOne_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Shipping.ConsiderAssociatedProductsDimensions")]
        public bool ConsiderAssociatedProductsDimensions { get; set; }
        public b
ool ConsiderAssociatedProductsDimensions_OverrideForStore { get; set; }

        public AddressModel ShippingOriginAddress { get; set; }
        public bool ShippingOriginAddress_OverrideForStore { get; set; }

        public string PrimaryStoreCurrencyCode { get; set; }

        #endregion
    }
}