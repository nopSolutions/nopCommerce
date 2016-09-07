using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Routing;
using Nop.Core.Domain.Shipping;
using Nop.Core.Plugins;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Shipping;
using Nop.Services.Shipping.Tracking;

namespace Nop.Plugin.Shipping.FixedRateShipping
{
    /// <summary>
    /// Fixed rate shipping computation method
    /// </summary>
    public class FixedRateShippingComputationMethod : BasePlugin, IShippingRateComputationMethod
    {
        #region Fields

        private readonly ISettingService _settingService;
        private readonly IShippingService _shippingService;

        #endregion

        #region Ctor
        public FixedRateShippingComputationMethod(ISettingService settingService,
            IShippingService shippingService)
        {
            this._settingService = settingService;
            this._shippingService = shippingService;
        }
        #endregion
        
        #region Utilities

        private decimal GetRate(int shippingMethodId)
        {
            string key = string.Format("ShippingRateComputationMethod.FixedRate.Rate.ShippingMethodId{0}", shippingMethodId);
            var rate = this._settingService.GetSettingByKey<decimal>(key);
            return rate;
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
                throw new ArgumentNullException("getShippingOptionRequest");

            var response = new GetShippingOptionResponse();

            if (getShippingOptionRequest.Items == null || !getShippingOptionRequest.Items.Any())
            {
                response.AddError("No shipment items");
                return response;
            }

            int? restrictByCountryId = (getShippingOptionRequest.ShippingAddress != null && getShippingOptionRequest.ShippingAddress.Country != null) ? (int?)getShippingOptionRequest.ShippingAddress.Country.Id : null;
            var shippingMethods = this._shippingService.GetAllShippingMethods(restrictByCountryId);
            foreach (var shippingMethod in shippingMethods)
            {
                var shippingOption = new ShippingOption();
                shippingOption.Name = shippingMethod.GetLocalized(x => x.Name);
                shippingOption.Description = shippingMethod.GetLocalized(x => x.Description);
                shippingOption.Rate = GetRate(shippingMethod.Id);
                response.ShippingOptions.Add(shippingOption);
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
                throw new ArgumentNullException("getShippingOptionRequest");

            int? restrictByCountryId = (getShippingOptionRequest.ShippingAddress != null && getShippingOptionRequest.ShippingAddress.Country != null) ? (int?)getShippingOptionRequest.ShippingAddress.Country.Id : null;
            var shippingMethods = this._shippingService.GetAllShippingMethods(restrictByCountryId);
            
            var rates = new List<decimal>();
            foreach (var shippingMethod in shippingMethods)
            {
                decimal rate = GetRate(shippingMethod.Id);
                if (!rates.Contains(rate))
                    rates.Add(rate);
            }

            //return default rate if all of them equal
            if (rates.Count == 1)
                return rates[0];

            return null;
        }

        /// <summary>
        /// Gets a route for provider configuration
        /// </summary>
        /// <param name="actionName">Action name</param>
        /// <param name="controllerName">Controller name</param>
        /// <param name="routeValues">Route values</param>
        public void GetConfigurationRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "Configure";
            controllerName = "ShippingFixedRate";
            routeValues = new RouteValueDictionary { { "Namespaces", "Nop.Plugin.Shipping.FixedRateShipping.Controllers" }, { "area", null } };
        }

        /// <summary>
        /// Install plugin
        /// </summary>
        public override void Install()
        {
            //locales
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.FixedRateShipping.Fields.ShippingMethodName", "Shipping method");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.FixedRateShipping.Fields.Rate", "Rate");

            base.Install();
        }


        /// <summary>
        /// Uninstall plugin
        /// </summary>
        public override void Uninstall()
        {
            //locales
            this.DeletePluginLocaleResource("Plugins.Shipping.FixedRateShipping.Fields.ShippingMethodName");
            this.DeletePluginLocaleResource("Plugins.Shipping.FixedRateShipping.Fields.Rate");

            base.Uninstall();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a shipping rate computation method type
        /// </summary>
        public ShippingRateComputationMethodType ShippingRateComputationMethodType
        {
            get
            {
                return ShippingRateComputationMethodType.Offline;
            }
        }
        
        /// <summary>
        /// Gets a shipment tracker
        /// </summary>
        public IShipmentTracker ShipmentTracker
        {
            get
            {
                //uncomment a line below to return a general shipment tracker (finds an appropriate tracker by tracking number)
                //return new GeneralShipmentTracker(EngineContext.Current.Resolve<ITypeFinder>());
                return null;
            }
        }

        #endregion
    }
}
