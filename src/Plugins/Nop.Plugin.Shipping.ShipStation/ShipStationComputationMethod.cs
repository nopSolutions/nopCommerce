using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the represents a response of getting shipping rate options
        /// </returns>
        public async Task<GetShippingOptionResponse> GetShippingOptionsAsync(GetShippingOptionRequest getShippingOptionRequest)
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
                foreach (var rate in await _shipStationService.GetAllRatesAsync(getShippingOptionRequest))
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
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the fixed shipping rate; or null in case there's no fixed shipping rate
        /// </returns>
        public Task<decimal?> GetFixedRateAsync(GetShippingOptionRequest getShippingOptionRequest)
        {
            if (getShippingOptionRequest == null)
                throw new ArgumentNullException(nameof(getShippingOptionRequest));

            return Task.FromResult<decimal?>(null);
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
        /// <returns>A task that represents the asynchronous operation</returns>
        public override async Task InstallAsync()
        {
            //settings
            var settings = new ShipStationSettings
            {
                PackingPackageVolume = 5184
            };
            await _settingService.SaveSettingAsync(settings);

            //locales
            await _localizationService.AddLocaleResourceAsync(new Dictionary<string, string>
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

            await base.InstallAsync();
        }

        /// <summary>
        /// Uninstall plugin
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        public override async Task UninstallAsync()
        {
            //settings
            await _settingService.DeleteSettingAsync<ShipStationSettings>();

            //locales
            await _localizationService.DeleteLocaleResourcesAsync("Enums.Nop.Plugin.Shipping.ShipStation");
            await _localizationService.DeleteLocaleResourcesAsync("Plugins.Shipping.ShipStation");

            await base.UninstallAsync();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a shipment tracker
        /// </summary>
        public IShipmentTracker ShipmentTracker => null;

        #endregion
    }
}