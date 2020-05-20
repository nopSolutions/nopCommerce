using System;
using System.Collections.Generic;
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
            _localizationService.AddPluginLocaleResource(new Dictionary<string, string>
            {
                ["Enums.Nop.Plugin.Shipping.ShipStation.PackingType.PackByDimensions"] = "Pack by dimensions",
                ["Enums.Nop.Plugin.Shipping.ShipStation.PackingType.PackByVolume"] = "Pack by volume",
                ["Plugins.Shipping.ShipStation.Fields.ApiKey.Hint"] = "Specify ShipStation API key.",
                ["Plugins.Shipping.ShipStation.Fields.ApiKey"] = "API key",
                ["Plugins.Shipping.ShipStation.Fields.ApiSecret.Hint"] = "Specify ShipStation API secret.",
                ["Plugins.Shipping.ShipStation.Fields.ApiSecret"] = "API secret",
                ["Plugins.Shipping.ShipStation.Fields.PackingPackageVolume.Hint"] = "Enter your package volume.",
                ["Plugins.Shipping.ShipStation.Fields.PackingPackageVolume"] = "Package volume",
                ["Plugins.Shipping.ShipStation.Fields.PackingType.Hint"] = "Choose preferred packing type.",
                ["Plugins.Shipping.ShipStation.Fields.PackingType"] = "Packing type",
                ["Plugins.Shipping.ShipStation.Fields.Password.Hint"] = "Specify ShipStation password",
                ["Plugins.Shipping.ShipStation.Fields.Password"] = "Password",
                ["Plugins.Shipping.ShipStation.Fields.PassDimensions.Hint"] = "Check if need send dimensions to the ShipStation server",
                ["Plugins.Shipping.ShipStation.Fields.PassDimensions"] = "Pass dimensions",
                ["Plugins.Shipping.ShipStation.Fields.UserName"] = "User name",
                ["Plugins.Shipping.ShipStation.Fields.UserName.Hint"] = "Specify ShipStation user name"
            });

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
            _localizationService.DeletePluginLocaleResources("Enums.Nop.Plugin.Shipping.ShipStation");
            _localizationService.DeletePluginLocaleResources("Plugins.Shipping.ShipStation");

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