using System;
using System.Linq;
using Nop.Core;
using Nop.Plugin.Shipping.UPS.Domain;
using Nop.Plugin.Shipping.UPS.Services;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Plugins;
using Nop.Services.Shipping;
using Nop.Services.Shipping.Tracking;

namespace Nop.Plugin.Shipping.UPS
{
    /// <summary>
    /// Represents UPS computation method
    /// </summary>
    public class UPSComputationMethod : BasePlugin, IShippingRateComputationMethod
    {
        #region Fields

        private readonly ILocalizationService _localizationService;
        private readonly ISettingService _settingService;
        private readonly IWebHelper _webHelper;
        private readonly UPSService _upsService;

        #endregion

        #region Ctor

        public UPSComputationMethod(ILocalizationService localizationService,
            ISettingService settingService,
            IWebHelper webHelper,
            UPSService upsService)
        {
            _localizationService = localizationService;
            _settingService = settingService;
            _webHelper = webHelper;
            _upsService = upsService;
        }

        #endregion

        #region Methods

        /// <summary>
        ///  Gets available shipping options
        /// </summary>
        /// <param name="getShippingOptionRequest">A request for getting shipping options</param>
        /// <returns>Represents a response of getting shipping rate options</returns>
        public GetShippingOptionResponse GetShippingOptions(GetShippingOptionRequest getShippingOptionRequest)
        {
            if (getShippingOptionRequest == null)
                throw new ArgumentNullException(nameof(getShippingOptionRequest));

            if (!getShippingOptionRequest.Items?.Any() ?? true)
                return new GetShippingOptionResponse { Errors = new[] { "No shipment items" } };

            if (getShippingOptionRequest.ShippingAddress?.Country == null)
                return new GetShippingOptionResponse { Errors = new[] { "Shipping address is not set" } };

            return _upsService.GetRates(getShippingOptionRequest);
        }

        /// <summary>
        /// Gets fixed shipping rate (if shipping rate computation method allows it and the rate can be calculated before checkout).
        /// </summary>
        /// <param name="getShippingOptionRequest">A request for getting shipping options</param>
        /// <returns>Fixed shipping rate; or null in case there's no fixed shipping rate</returns>
        public decimal? GetFixedRate(GetShippingOptionRequest getShippingOptionRequest)
        {
            return null;
        }

        /// <summary>
        /// Gets a configuration page URL
        /// </summary>
        public override string GetConfigurationPageUrl()
        {
            return $"{_webHelper.GetStoreLocation()}Admin/UPSShipping/Configure";
        }

        /// <summary>
        /// Install plugin
        /// </summary>
        public override void Install()
        {
            //settings
            _settingService.SaveSetting(new UPSSettings
            {
                UseSandbox = true,
                CustomerClassification = CustomerClassification.StandardListRates,
                PickupType = PickupType.OneTimePickup,
                PackagingType = PackagingType.ExpressBox,
                PackingPackageVolume = 5184,
                PackingType = PackingType.PackByDimensions,
                PassDimensions = true
            });

            //locales
            _localizationService.AddOrUpdatePluginLocaleResource("Enums.Nop.Plugin.Shipping.UPS.PackingType.PackByDimensions", "Pack by dimensions");
            _localizationService.AddOrUpdatePluginLocaleResource("Enums.Nop.Plugin.Shipping.UPS.PackingType.PackByOneItemPerPackage", "Pack by one item per package");
            _localizationService.AddOrUpdatePluginLocaleResource("Enums.Nop.Plugin.Shipping.UPS.PackingType.PackByVolume", "Pack by volume");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Shipping.UPS.Fields.AccessKey", "Access Key");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Shipping.UPS.Fields.AccessKey.Hint", "Specify UPS access key.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Shipping.UPS.Fields.AccountNumber", "Account number");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Shipping.UPS.Fields.AccountNumber.Hint", "Specify UPS account number (required to get negotiated rates).");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Shipping.UPS.Fields.AdditionalHandlingCharge", "Additional handling charge");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Shipping.UPS.Fields.AdditionalHandlingCharge.Hint", "Enter additional handling fee to charge your customers.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Shipping.UPS.Fields.AvailableCarrierServices", "Carrier Services");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Shipping.UPS.Fields.AvailableCarrierServices.Hint", "Select the services you want to offer to customers.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Shipping.UPS.Fields.CustomerClassification", "UPS Customer Classification");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Shipping.UPS.Fields.CustomerClassification.Hint", "Choose customer classification.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Shipping.UPS.Fields.InsurePackage", "Insure package");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Shipping.UPS.Fields.InsurePackage.Hint", "Check to insure packages.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Shipping.UPS.Fields.PackagingType", "UPS Packaging Type");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Shipping.UPS.Fields.PackagingType.Hint", "Choose UPS packaging type.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Shipping.UPS.Fields.PackingPackageVolume", "Package volume");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Shipping.UPS.Fields.PackingPackageVolume.Hint", "Enter your package volume.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Shipping.UPS.Fields.PackingType", "Packing type");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Shipping.UPS.Fields.PackingType.Hint", "Choose preferred packing type.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Shipping.UPS.Fields.PassDimensions", "Pass dimensions");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Shipping.UPS.Fields.PassDimensions.Hint", "Check if you want to pass package dimensions when requesting rates.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Shipping.UPS.Fields.Password", "Password");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Shipping.UPS.Fields.Password.Hint", "Specify UPS password.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Shipping.UPS.Fields.PickupType", "UPS Pickup Type");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Shipping.UPS.Fields.PickupType.Hint", "Choose UPS pickup type.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Shipping.UPS.Fields.SaturdayDeliveryEnabled", "Saturday Delivery enabled");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Shipping.UPS.Fields.SaturdayDeliveryEnabled.Hint", "Check to get rates for Saturday Delivery options.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Shipping.UPS.Fields.Tracing", "Tracing");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Shipping.UPS.Fields.Tracing.Hint", "Check if you want to record plugin tracing in System Log. Warning: The entire request and response XML will be logged (including AccessKey/Username,Password). Do not leave this enabled in a production environment.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Shipping.UPS.Fields.Username", "Username");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Shipping.UPS.Fields.Username.Hint", "Specify UPS username.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Shipping.UPS.Fields.UseSandbox", "Use sandbox");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Shipping.UPS.Fields.UseSandbox.Hint", "Check to use sandbox (testing environment).");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Shipping.UPS.Tracker.Arrived", "Arrived");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Shipping.UPS.Tracker.Booked", "Booked");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Shipping.UPS.Tracker.Delivered", "Delivered");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Shipping.UPS.Tracker.Departed", "Departed");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Shipping.UPS.Tracker.ExportScanned", "Export scanned");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Shipping.UPS.Tracker.NotDelivered", "Not delivered");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Shipping.UPS.Tracker.OriginScanned", "Origin scanned");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Shipping.UPS.Tracker.Pickup", "Pickup");

            base.Install();
        }

        /// <summary>
        /// Uninstall plugin
        /// </summary>
        public override void Uninstall()
        {
            //settings
            _settingService.DeleteSetting<UPSSettings>();

            //locales
            _localizationService.DeletePluginLocaleResource("Enums.Nop.Plugin.Shipping.UPS.PackingType.PackByDimensions");
            _localizationService.DeletePluginLocaleResource("Enums.Nop.Plugin.Shipping.UPS.PackingType.PackByOneItemPerPackage");
            _localizationService.DeletePluginLocaleResource("Enums.Nop.Plugin.Shipping.UPS.PackingType.PackByVolume");
            _localizationService.DeletePluginLocaleResource("Plugins.Shipping.UPS.Fields.AccessKey");
            _localizationService.DeletePluginLocaleResource("Plugins.Shipping.UPS.Fields.AccessKey.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Shipping.UPS.Fields.AccountNumber");
            _localizationService.DeletePluginLocaleResource("Plugins.Shipping.UPS.Fields.AccountNumber.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Shipping.UPS.Fields.AdditionalHandlingCharge");
            _localizationService.DeletePluginLocaleResource("Plugins.Shipping.UPS.Fields.AdditionalHandlingCharge.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Shipping.UPS.Fields.AvailableCarrierServices");
            _localizationService.DeletePluginLocaleResource("Plugins.Shipping.UPS.Fields.AvailableCarrierServices.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Shipping.UPS.Fields.CustomerClassification");
            _localizationService.DeletePluginLocaleResource("Plugins.Shipping.UPS.Fields.CustomerClassification.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Shipping.UPS.Fields.InsurePackage");
            _localizationService.DeletePluginLocaleResource("Plugins.Shipping.UPS.Fields.InsurePackage.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Shipping.UPS.Fields.PackagingType");
            _localizationService.DeletePluginLocaleResource("Plugins.Shipping.UPS.Fields.PackagingType.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Shipping.UPS.Fields.PackingPackageVolume");
            _localizationService.DeletePluginLocaleResource("Plugins.Shipping.UPS.Fields.PackingPackageVolume.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Shipping.UPS.Fields.PackingType");
            _localizationService.DeletePluginLocaleResource("Plugins.Shipping.UPS.Fields.PackingType.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Shipping.UPS.Fields.PassDimensions");
            _localizationService.DeletePluginLocaleResource("Plugins.Shipping.UPS.Fields.PassDimensions.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Shipping.UPS.Fields.Password");
            _localizationService.DeletePluginLocaleResource("Plugins.Shipping.UPS.Fields.Password.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Shipping.UPS.Fields.PickupType");
            _localizationService.DeletePluginLocaleResource("Plugins.Shipping.UPS.Fields.PickupType.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Shipping.UPS.Fields.SaturdayDeliveryEnabled");
            _localizationService.DeletePluginLocaleResource("Plugins.Shipping.UPS.Fields.SaturdayDeliveryEnabled.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Shipping.UPS.Fields.Tracing");
            _localizationService.DeletePluginLocaleResource("Plugins.Shipping.UPS.Fields.Tracing.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Shipping.UPS.Fields.Username");
            _localizationService.DeletePluginLocaleResource("Plugins.Shipping.UPS.Fields.Username.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Shipping.UPS.Fields.UseSandbox");
            _localizationService.DeletePluginLocaleResource("Plugins.Shipping.UPS.Fields.UseSandbox.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Shipping.UPS.Tracker.Arrived");
            _localizationService.DeletePluginLocaleResource("Plugins.Shipping.UPS.Tracker.Booked");
            _localizationService.DeletePluginLocaleResource("Plugins.Shipping.UPS.Tracker.Delivered");
            _localizationService.DeletePluginLocaleResource("Plugins.Shipping.UPS.Tracker.Departed");
            _localizationService.DeletePluginLocaleResource("Plugins.Shipping.UPS.Tracker.ExportScanned");
            _localizationService.DeletePluginLocaleResource("Plugins.Shipping.UPS.Tracker.NotDelivered");
            _localizationService.DeletePluginLocaleResource("Plugins.Shipping.UPS.Tracker.OriginScanned");
            _localizationService.DeletePluginLocaleResource("Plugins.Shipping.UPS.Tracker.Pickup");

            base.Uninstall();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a shipping rate computation method type
        /// </summary>
        public ShippingRateComputationMethodType ShippingRateComputationMethodType => ShippingRateComputationMethodType.Realtime;

        /// <summary>
        /// Gets a shipment tracker
        /// </summary>
        public IShipmentTracker ShipmentTracker => new UPSShipmentTracker(_upsService);

        #endregion
    }
}