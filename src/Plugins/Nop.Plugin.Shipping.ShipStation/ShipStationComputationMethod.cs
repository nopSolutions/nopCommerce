using System;
using Nop.Core;
using Nop.Core.Domain.Shipping;
using Nop.Plugin.Shipping.ShipStation.Services;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Plugins;
using Nop.Services.Shipping;
using Nop.Services.Shipping.Tracking;

namespace Nop.Plugin.Shipping.ShipStation
{
    /// <summary>
    /// Fixed rate or by weight shipping computation method 
    /// </summary>
    public class ShipStationComputationMethod : BasePlugin, IShippingRateComputationMethod, IMiscPlugin
    {
        #region Fields
       
        private readonly ILocalizationService _localizationService;
        private readonly ISettingService _settingService;
        private readonly IShipStationService _shipStationService;
        private readonly IWebHelper _webHelper;

        #endregion

        #region Ctor

        public ShipStationComputationMethod(ILocalizationService localizationService,
            ISettingService settingService,
            IShipStationService shipStationService,
            IWebHelper webHelper)
        {
            _localizationService = localizationService;
            _settingService = settingService;
            _shipStationService = shipStationService;
            _webHelper = webHelper;
        }

        #endregion

        #region Utilities

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

            var response = new GetShippingOptionResponse();

            if (getShippingOptionRequest.Items == null)
                response.AddError("No shipment items");

            if (getShippingOptionRequest.ShippingAddress == null)
                response.AddError("Shipping address is not set");

            if ((getShippingOptionRequest.ShippingAddress?.CountryId ?? 0) == 0)
                response.AddError("Shipping country is not set");

            if (!response.Success)
                return response;

            try
            {
                foreach (var rate in _shipStationService.GetAllRates(getShippingOptionRequest))
                {
                    response.ShippingOptions.Add(new ShippingOption
                    {
                        Description = rate.ServiceCode,
                        Name = rate.ServiceName,
                        Rate = rate.TotalCost
                    });
                }
            }
            catch (Exception e)
            {
                response.Errors.Add(e.Message);
            }

            return response;
        }

        /// <summary>
        /// Gets fixed shipping rate (if shipping rate computation method allows it and the rate can be calculated before checkout).
        /// </summary>
        /// <param name="getShippingOptionRequest">A request for getting shipping options</param>
        /// <returns>Fixed shipping rate; or null in case there's no fixed shipping rate</returns>
        public decimal? GetFixedRate(GetShippingOptionRequest getShippingOptionRequest)
        {
            if (getShippingOptionRequest == null)
                throw new ArgumentNullException(nameof(getShippingOptionRequest));

            return null;
        }

        /// <summary>
        /// Gets a configuration page URL
        /// </summary>
        public override string GetConfigurationPageUrl()
        {
            return $"{_webHelper.GetStoreLocation()}Admin/ShipStation/Configure";
        }

        /// <summary>
        /// Install plugin
        /// </summary>
        public override void Install()
        {
            //settings
            var settings = new ShipStationSettings
            {
                PackingPackageVolume = 5184
            };
            _settingService.SaveSetting(settings);

            //locales
            _localizationService.AddOrUpdatePluginLocaleResource("Enums.Nop.Plugin.Shipping.ShipStation.PackingType.PackByDimensions", "Pack by dimensions");
            _localizationService.AddOrUpdatePluginLocaleResource("Enums.Nop.Plugin.Shipping.ShipStation.PackingType.PackByVolume", "Pack by volume");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Shipping.ShipStation.Fields.ApiKey.Hint", "Specify ShipStation API key.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Shipping.ShipStation.Fields.ApiKey", "API key");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Shipping.ShipStation.Fields.ApiSecret.Hint", "Specify ShipStation API secret.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Shipping.ShipStation.Fields.ApiSecret", "API secret");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Shipping.ShipStation.Fields.PackingPackageVolume.Hint", "Enter your package volume.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Shipping.ShipStation.Fields.PackingPackageVolume", "Package volume");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Shipping.ShipStation.Fields.PackingType.Hint", "Choose preferred packing type.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Shipping.ShipStation.Fields.PackingType", "Packing type");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Shipping.ShipStation.Fields.Password.Hint", "Specify ShipStation password");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Shipping.ShipStation.Fields.Password", "Password");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Shipping.ShipStation.Fields.PassDimensions.Hint", "Check if need send dimensions to the ShipStation server");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Shipping.ShipStation.Fields.PassDimensions", "Pass dimensions");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Shipping.ShipStation.Fields.UserName", "User name");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Shipping.ShipStation.Fields.UserName.Hint", "Specify ShipStation user name");

            base.Install();
        }

        /// <summary>
        /// Uninstall plugin
        /// </summary>
        public override void Uninstall()
        {
            //settings
            _settingService.DeleteSetting<ShipStationSettings>();

            //locales
            _localizationService.DeletePluginLocaleResource("Enums.Nop.Plugin.Shipping.ShipStation.PackingType.PackByDimensions");
            _localizationService.DeletePluginLocaleResource("Enums.Nop.Plugin.Shipping.ShipStation.PackingType.PackByVolume");
            _localizationService.DeletePluginLocaleResource("Plugins.Shipping.ShipStation.Fields.ApiKey.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Shipping.ShipStation.Fields.ApiKey");
            _localizationService.DeletePluginLocaleResource("Plugins.Shipping.ShipStation.Fields.ApiSecret.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Shipping.ShipStation.Fields.ApiSecret");
            _localizationService.DeletePluginLocaleResource("Plugins.Shipping.ShipStation.Fields.PackingPackageVolume.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Shipping.ShipStation.Fields.PackingPackageVolume");
            _localizationService.DeletePluginLocaleResource("Plugins.Shipping.ShipStation.Fields.PackingType.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Shipping.ShipStation.Fields.PackingType");
            _localizationService.DeletePluginLocaleResource("Plugins.Shipping.ShipStation.Fields.Password.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Shipping.ShipStation.Fields.Password");
            _localizationService.DeletePluginLocaleResource("Plugins.Shipping.ShipStation.Fields.PassDimensions.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Shipping.ShipStation.Fields.PassDimensions");
            _localizationService.DeletePluginLocaleResource("Plugins.Shipping.ShipStation.Fields.UserName.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Shipping.ShipStation.Fields.UserName");

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
        public IShipmentTracker ShipmentTracker => null;

        #endregion
    }
}