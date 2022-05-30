using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the represents a response of getting shipping rate options
        /// </returns>
        public async Task<GetShippingOptionResponse> GetShippingOptionsAsync(GetShippingOptionRequest getShippingOptionRequest)
        {
            if (getShippingOptionRequest == null)
                throw new ArgumentNullException(nameof(getShippingOptionRequest));

            if (!getShippingOptionRequest.Items?.Any() ?? true)
                return new GetShippingOptionResponse { Errors = new[] { "No shipment items" } };

            if (getShippingOptionRequest.ShippingAddress?.CountryId == null)
                return new GetShippingOptionResponse { Errors = new[] { "Shipping address is not set" } };

            return await _upsService.GetRatesAsync(getShippingOptionRequest);
        }

        /// <summary>
        /// Gets fixed shipping rate (if shipping rate computation method allows it and the rate can be calculated before checkout).
        /// </summary>
        /// <param name="getShippingOptionRequest">A request for getting shipping options</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the fixed shipping rate; or null in case there's no fixed shipping rate
        /// </returns>
        public Task<decimal?> GetFixedRateAsync(GetShippingOptionRequest getShippingOptionRequest)
        {
            return Task.FromResult<decimal?>(null);
        }

        /// <summary>
        /// Get associated shipment tracker
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the shipment tracker
        /// </returns>
        public Task<IShipmentTracker> GetShipmentTrackerAsync()
        {
            return Task.FromResult<IShipmentTracker>(new UPSShipmentTracker(_upsService));
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
        /// <returns>A task that represents the asynchronous operation</returns>
        public override async Task InstallAsync()
        {
            //settings
            await _settingService.SaveSettingAsync(new UPSSettings
            {
                UseSandbox = true,
                CustomerClassification = CustomerClassification.StandardListRates,
                PickupType = PickupType.OneTimePickup,
                PackagingType = PackagingType.ExpressBox,
                PackingPackageVolume = 5184,
                PackingType = PackingType.PackByDimensions,
                PassDimensions = true,
                WeightType = "LBS",
                DimensionsType = "IN"
            });

            //locales
            await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
            {
                ["Enums.Nop.Plugin.Shipping.UPS.PackingType.PackByDimensions"] = "Pack by dimensions",
                ["Enums.Nop.Plugin.Shipping.UPS.PackingType.PackByOneItemPerPackage"] = "Pack by one item per package",
                ["Enums.Nop.Plugin.Shipping.UPS.PackingType.PackByVolume"] = "Pack by volume",
                ["Plugins.Shipping.UPS.Fields.AccessKey"] = "Access Key",
                ["Plugins.Shipping.UPS.Fields.AccessKey.Hint"] = "Specify UPS access key.",
                ["Plugins.Shipping.UPS.Fields.AccountNumber"] = "Account number",
                ["Plugins.Shipping.UPS.Fields.AccountNumber.Hint"] = "Specify UPS account number (required to get negotiated rates).",
                ["Plugins.Shipping.UPS.Fields.AdditionalHandlingCharge"] = "Additional handling charge",
                ["Plugins.Shipping.UPS.Fields.AdditionalHandlingCharge.Hint"] = "Enter additional handling fee to charge your customers.",
                ["Plugins.Shipping.UPS.Fields.AvailableCarrierServices"] = "Carrier Services",
                ["Plugins.Shipping.UPS.Fields.AvailableCarrierServices.Hint"] = "Select the services you want to offer to customers.",
                ["Plugins.Shipping.UPS.Fields.CustomerClassification"] = "UPS Customer Classification",
                ["Plugins.Shipping.UPS.Fields.CustomerClassification.Hint"] = "Choose customer classification.",
                ["Plugins.Shipping.UPS.Fields.DimensionsType"] = "Dimensions type",
                ["Plugins.Shipping.UPS.Fields.DimensionsType.Hint"] = "Choose dimensions type (inches or centimeters).",
                ["Plugins.Shipping.UPS.Fields.InsurePackage"] = "Insure package",
                ["Plugins.Shipping.UPS.Fields.InsurePackage.Hint"] = "Check to insure packages.",
                ["Plugins.Shipping.UPS.Fields.PackagingType"] = "UPS Packaging Type",
                ["Plugins.Shipping.UPS.Fields.PackagingType.Hint"] = "Choose UPS packaging type.",
                ["Plugins.Shipping.UPS.Fields.PackingPackageVolume"] = "Package volume",
                ["Plugins.Shipping.UPS.Fields.PackingPackageVolume.Hint"] = "Enter your package volume.",
                ["Plugins.Shipping.UPS.Fields.PackingType"] = "Packing type",
                ["Plugins.Shipping.UPS.Fields.PackingType.Hint"] = "Choose preferred packing type.",
                ["Plugins.Shipping.UPS.Fields.PassDimensions"] = "Pass dimensions",
                ["Plugins.Shipping.UPS.Fields.PassDimensions.Hint"] = "Check if you want to pass package dimensions when requesting rates.",
                ["Plugins.Shipping.UPS.Fields.Password"] = "Password",
                ["Plugins.Shipping.UPS.Fields.Password.Hint"] = "Specify UPS password.",
                ["Plugins.Shipping.UPS.Fields.PickupType"] = "UPS Pickup Type",
                ["Plugins.Shipping.UPS.Fields.PickupType.Hint"] = "Choose UPS pickup type.",
                ["Plugins.Shipping.UPS.Fields.SaturdayDeliveryEnabled"] = "Saturday Delivery enabled",
                ["Plugins.Shipping.UPS.Fields.SaturdayDeliveryEnabled.Hint"] = "Check to get rates for Saturday Delivery options.",
                ["Plugins.Shipping.UPS.Fields.Tracing"] = "Tracing",
                ["Plugins.Shipping.UPS.Fields.Tracing.Hint"] = "Check if you want to record plugin tracing in System Log. Warning: The entire request and response XML will be logged (including AccessKey/Username,Password). Do not leave this enabled in a production environment.",
                ["Plugins.Shipping.UPS.Fields.Username"] = "Username",
                ["Plugins.Shipping.UPS.Fields.Username.Hint"] = "Specify UPS username.",
                ["Plugins.Shipping.UPS.Fields.UseSandbox"] = "Use sandbox",
                ["Plugins.Shipping.UPS.Fields.UseSandbox.Hint"] = "Check to use sandbox (testing environment).",
                ["Plugins.Shipping.UPS.Fields.WeightType"] = "Weight type",
                ["Plugins.Shipping.UPS.Fields.WeightType.Hint"] = "Choose the weight type (pounds or kilograms).",
                ["Plugins.Shipping.UPS.Tracker.Arrived"] = "Arrived",
                ["Plugins.Shipping.UPS.Tracker.Booked"] = "Booked",
                ["Plugins.Shipping.UPS.Tracker.Delivered"] = "Delivered",
                ["Plugins.Shipping.UPS.Tracker.Departed"] = "Departed",
                ["Plugins.Shipping.UPS.Tracker.ExportScanned"] = "Export scanned",
                ["Plugins.Shipping.UPS.Tracker.NotDelivered"] = "Not delivered",
                ["Plugins.Shipping.UPS.Tracker.OriginScanned"] = "Origin scanned",
                ["Plugins.Shipping.UPS.Tracker.Pickup"] = "Pickup"
            });

            await base.InstallAsync();
        }

        /// <summary>
        /// Uninstall plugin
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        public override async Task UninstallAsync()
        {
            //settings
            await _settingService.DeleteSettingAsync<UPSSettings>();

            //locales
            await _localizationService.DeleteLocaleResourcesAsync("Enums.Nop.Plugin.Shipping.UPS");
            await _localizationService.DeleteLocaleResourcesAsync("Plugins.Shipping.UPS");

            await base.UninstallAsync();
        }

        #endregion
    }
}