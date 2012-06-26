using System;
using System.Web.Routing;
using Nop.Core.Domain.Shipping;
using Nop.Core.Plugins;
using Nop.Plugin.Shipping.ByWeight.Data;
using Nop.Plugin.Shipping.ByWeight.Services;
using Nop.Services.Catalog;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Shipping;
using Nop.Services.Shipping.Tracking;

namespace Nop.Plugin.Shipping.ByWeight
{
    public class ByWeightShippingComputationMethod : BasePlugin, IShippingRateComputationMethod
    {
        #region Fields

        private readonly IShippingService _shippingService;
        private readonly IShippingByWeightService _shippingByWeightService;
        private readonly IPriceCalculationService _priceCalculationService;
        private readonly ShippingByWeightSettings _shippingByWeightSettings;
        private readonly ShippingByWeightObjectContext _objectContext;
        private readonly ISettingService _settingService;

        #endregion

        #region Ctor
        public ByWeightShippingComputationMethod(IShippingService shippingService,
            IShippingByWeightService shippingByWeightService,
            IPriceCalculationService priceCalculationService, 
            ShippingByWeightSettings shippingByWeightSettings,
            ShippingByWeightObjectContext objectContext,
            ISettingService settingService)
        {
            this._shippingService = shippingService;
            this._shippingByWeightService = shippingByWeightService;
            this._priceCalculationService = priceCalculationService;
            this._shippingByWeightSettings = shippingByWeightSettings;
            this._objectContext = objectContext;
            this._settingService = settingService;
        }
        #endregion

        #region Utilities
        
        private decimal? GetRate(decimal subTotal, decimal weight, int shippingMethodId,
            int countryId, int stateProvinceId, string zip)
        {
            decimal? shippingTotal = null;

            var shippingByWeightRecord = _shippingByWeightService.FindRecord(shippingMethodId, 
                countryId, stateProvinceId, zip, weight);
            if (shippingByWeightRecord == null)
            {
                if (_shippingByWeightSettings.LimitMethodsToCreated)
                    return null;
                else
                    return decimal.Zero;
            }
            if (shippingByWeightRecord.UsePercentage && shippingByWeightRecord.ShippingChargePercentage <= decimal.Zero)
                return decimal.Zero;
            if (!shippingByWeightRecord.UsePercentage && shippingByWeightRecord.ShippingChargeAmount <= decimal.Zero)
                return decimal.Zero;
            if (shippingByWeightRecord.UsePercentage)
                shippingTotal = Math.Round((decimal)((((float)subTotal) * ((float)shippingByWeightRecord.ShippingChargePercentage)) / 100f), 2);
            else
            {
                if (_shippingByWeightSettings.CalculatePerWeightUnit)
                    shippingTotal = shippingByWeightRecord.ShippingChargeAmount * weight;
                else
                    shippingTotal = shippingByWeightRecord.ShippingChargeAmount;
            }
            if (shippingTotal < decimal.Zero)
                shippingTotal = decimal.Zero;
            return shippingTotal;
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

            if (getShippingOptionRequest.Items == null || getShippingOptionRequest.Items.Count == 0)
            {
                response.AddError("No shipment items");
                return response;
            }
            if (getShippingOptionRequest.ShippingAddress == null)
            {
                response.AddError("Shipping address is not set");
                return response;
            }
            
            int countryId = getShippingOptionRequest.ShippingAddress.CountryId.HasValue ? getShippingOptionRequest.ShippingAddress.CountryId.Value : 0;
            int stateProvinceId = getShippingOptionRequest.ShippingAddress.StateProvinceId.HasValue ? getShippingOptionRequest.ShippingAddress.StateProvinceId.Value : 0;
            string zip = getShippingOptionRequest.ShippingAddress.ZipPostalCode;
            decimal subTotal = decimal.Zero;
            foreach (var shoppingCartItem in getShippingOptionRequest.Items)
            {
                if (shoppingCartItem.IsFreeShipping || !shoppingCartItem.IsShipEnabled)
                    continue;
                subTotal += _priceCalculationService.GetSubTotal(shoppingCartItem, true);
            }
            decimal weight = _shippingService.GetShoppingCartTotalWeight(getShippingOptionRequest.Items);

            var shippingMethods = _shippingService.GetAllShippingMethods(countryId);
            foreach (var shippingMethod in shippingMethods)
            {
                decimal? rate = GetRate(subTotal, weight, shippingMethod.Id,
                    countryId, stateProvinceId, zip);
                if (rate.HasValue)
                {
                    var shippingOption = new ShippingOption();
                    shippingOption.Name = shippingMethod.GetLocalized(x => x.Name);
                    shippingOption.Description = shippingMethod.GetLocalized(x => x.Description);
                    shippingOption.Rate = rate.Value;
                    response.ShippingOptions.Add(shippingOption);
                }
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
            controllerName = "ShippingByWeight";
            routeValues = new RouteValueDictionary() { { "Namespaces", "Nop.Plugin.Shipping.ByWeight.Controllers" }, { "area", null } };
        }
        
        /// <summary>
        /// Install plugin
        /// </summary>
        public override void Install()
        {
            //settings
            var settings = new ShippingByWeightSettings()
            {
                CalculatePerWeightUnit = false,
                LimitMethodsToCreated = false,
            };
            _settingService.SaveSetting(settings);


            //database objects
            _objectContext.Install();

            //locales
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.ByWeight.Fields.Country", "Country");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.ByWeight.Fields.Country.Hint", "If an asterisk is selected, then this shipping rate will apply to all customers, regardless of the country.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.ByWeight.Fields.StateProvince", "State / province");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.ByWeight.Fields.StateProvince.Hint", "If an asterisk is selected, then this shipping rate will apply to all customers from the given country, regardless of the state.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.ByWeight.Fields.Zip", "Zip");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.ByWeight.Fields.Zip.Hint", "Zip / postal code. If zip is empty, then this shipping rate will apply to all customers from the given country or state, regardless of the zip code.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.ByWeight.Fields.ShippingMethod", "Shipping method");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.ByWeight.Fields.ShippingMethod.Hint", "The shipping method.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.ByWeight.Fields.From", "Order weight from");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.ByWeight.Fields.From.Hint", "Order weight from.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.ByWeight.Fields.To", "Order weight to");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.ByWeight.Fields.To.Hint", "Order weight toy.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.ByWeight.Fields.UsePercentage", "Use percentage");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.ByWeight.Fields.UsePercentage.Hint", "Check to use 'charge percentage' value.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.ByWeight.Fields.ShippingChargePercentage", "Charge percentage (of subtotal)");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.ByWeight.Fields.ShippingChargePercentage.Hint", "Charge percentage (of subtotal).");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.ByWeight.Fields.ShippingChargeAmount", "Charge amount");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.ByWeight.Fields.ShippingChargeAmount.Hint", "Charge amount.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.ByWeight.Fields.LimitMethodsToCreated", "Limit shipping methods to configured ones");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.ByWeight.Fields.LimitMethodsToCreated.Hint", "If you check this option, then your customers will be limited to shipping options configured here. Otherwise, they'll be able to choose any existing shipping options even they've not configured here (zero shipping fee in this case).");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.ByWeight.Fields.CalculatePerWeightUnit", "Calculate per weight unit");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.ByWeight.Fields.CalculatePerWeightUnit.Hint", "If you check this option, then rates are multiplied per weight unit (lb, kg, etc). This option is used for the fixed rates (without percents).");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.ByWeight.AddRecord", "Add record");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.ByWeight.AddRecord.Hint", "Adding a new record");
            
            base.Install();
        }

        /// <summary>
        /// Uninstall plugin
        /// </summary>
        public override void Uninstall()
        {
            //settings
            _settingService.DeleteSetting<ShippingByWeightSettings>();

            //database objects
            _objectContext.Uninstall();

            //locales
            this.DeletePluginLocaleResource("Plugins.Shipping.ByWeight.Fields.Country");
            this.DeletePluginLocaleResource("Plugins.Shipping.ByWeight.Fields.Country.Hint");
            this.DeletePluginLocaleResource("Plugins.Shipping.ByWeight.Fields.StateProvince");
            this.DeletePluginLocaleResource("Plugins.Shipping.ByWeight.Fields.StateProvince.Hint");
            this.DeletePluginLocaleResource("Plugins.Shipping.ByWeight.Fields.Zip");
            this.DeletePluginLocaleResource("Plugins.Shipping.ByWeight.Fields.Zip.Hint");
            this.DeletePluginLocaleResource("Plugins.Shipping.ByWeight.Fields.ShippingMethod");
            this.DeletePluginLocaleResource("Plugins.Shipping.ByWeight.Fields.ShippingMethod.Hint");
            this.DeletePluginLocaleResource("Plugins.Shipping.ByWeight.Fields.From");
            this.DeletePluginLocaleResource("Plugins.Shipping.ByWeight.Fields.From.Hint");
            this.DeletePluginLocaleResource("Plugins.Shipping.ByWeight.Fields.To");
            this.DeletePluginLocaleResource("Plugins.Shipping.ByWeight.Fields.To.Hint");
            this.DeletePluginLocaleResource("Plugins.Shipping.ByWeight.Fields.UsePercentage");
            this.DeletePluginLocaleResource("Plugins.Shipping.ByWeight.Fields.UsePercentage.Hint");
            this.DeletePluginLocaleResource("Plugins.Shipping.ByWeight.Fields.ShippingChargePercentage");
            this.DeletePluginLocaleResource("Plugins.Shipping.ByWeight.Fields.ShippingChargePercentage.Hint");
            this.DeletePluginLocaleResource("Plugins.Shipping.ByWeight.Fields.ShippingChargeAmount");
            this.DeletePluginLocaleResource("Plugins.Shipping.ByWeight.Fields.ShippingChargeAmount.Hint");
            this.DeletePluginLocaleResource("Plugins.Shipping.ByWeight.Fields.LimitMethodsToCreated");
            this.DeletePluginLocaleResource("Plugins.Shipping.ByWeight.Fields.LimitMethodsToCreated.Hint");
            this.DeletePluginLocaleResource("Plugins.Shipping.ByWeight.Fields.CalculatePerWeightUnit");
            this.DeletePluginLocaleResource("Plugins.Shipping.ByWeight.Fields.CalculatePerWeightUnit.Hint");
            this.DeletePluginLocaleResource("Plugins.Shipping.ByWeight.AddRecord");
            this.DeletePluginLocaleResource("Plugins.Shipping.ByWeight.AddRecord.Hint");
            
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
