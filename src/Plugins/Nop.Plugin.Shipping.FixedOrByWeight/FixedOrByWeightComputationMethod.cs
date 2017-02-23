using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Routing;
using Nop.Core;
using Nop.Core.Domain.Shipping;
using Nop.Core.Plugins;
using Nop.Plugin.Shipping.FixedOrByWeight.Data;
using Nop.Plugin.Shipping.FixedOrByWeight.Services;
using Nop.Services.Catalog;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Shipping;
using Nop.Services.Shipping.Tracking;

namespace Nop.Plugin.Shipping.FixedOrByWeight
{
    /// <summary>
    /// Fixed rate or by weight shipping computation method 
    /// </summary>
    public class FixedOrByWeightComputationMethod : BasePlugin, IShippingRateComputationMethod
    {
        #region Fields

        private readonly ISettingService _settingService;
        private readonly IShippingService _shippingService;
        private readonly IShippingByWeightService _shippingByWeightService;
        private readonly FixedOrByWeightSettings _fixedOrByWeightSettings;
        private readonly IStoreContext _storeContext;
        private readonly IPriceCalculationService _priceCalculationService;
        private readonly ShippingByWeightObjectContext _objectContext;

        #endregion

        #region Ctor

        public FixedOrByWeightComputationMethod(ISettingService settingService,
            IShippingService shippingService,
            FixedOrByWeightSettings fixedOrByWeightSettings,
            IShippingByWeightService shippingByWeightService,
            IStoreContext storeContext,
            IPriceCalculationService priceCalculationService,
        ShippingByWeightObjectContext objectContext)
        {
            this._settingService = settingService;
            this._shippingService = shippingService;
            this._fixedOrByWeightSettings = fixedOrByWeightSettings;
            this._shippingByWeightService = shippingByWeightService;
            this._storeContext = storeContext;
            this._priceCalculationService = priceCalculationService;
            this._objectContext = objectContext;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Get fixed rate
        /// </summary>
        /// <param name="shippingMethodId">Shipping method ID</param>
        /// <returns>Rate</returns>
        private decimal GetRate(int shippingMethodId)
        {
            var key = string.Format("ShippingRateComputationMethod.FixedOrByWeight.Rate.ShippingMethodId{0}", shippingMethodId);
            var rate = _settingService.GetSettingByKey<decimal>(key);
            return rate;
        }

        /// <summary>
        /// Get rate by weight
        /// </summary>
        /// <param name="subTotal">Subtotal</param>
        /// <param name="weight">Weight</param>
        /// <param name="shippingMethodId">Shipping method ID</param>
        /// <param name="storeId">Store ID</param>
        /// <param name="warehouseId">Warehouse ID</param>
        /// <param name="countryId">Country ID</param>
        /// <param name="stateProvinceId">State/Province ID</param>
        /// <param name="zip">Zip code</param>
        /// <returns>Rate</returns>
        private decimal? GetRate(decimal subTotal, decimal weight, int shippingMethodId,
            int storeId, int warehouseId, int countryId, int stateProvinceId, string zip)
        {
            var shippingByWeightRecord = _shippingByWeightService.FindRecord(shippingMethodId,
                storeId, warehouseId, countryId, stateProvinceId, zip, weight);
            if (shippingByWeightRecord == null)
            {
                if (_fixedOrByWeightSettings.LimitMethodsToCreated)
                    return null;

                return decimal.Zero;
            }

            //additional fixed cost
            var shippingTotal = shippingByWeightRecord.AdditionalFixedCost;
            //charge amount per weight unit
            if (shippingByWeightRecord.RatePerWeightUnit > decimal.Zero)
            {
                var weightRate = weight - shippingByWeightRecord.LowerWeightLimit;
                if (weightRate < decimal.Zero)
                    weightRate = decimal.Zero;
                shippingTotal += shippingByWeightRecord.RatePerWeightUnit * weightRate;
            }
            //percentage rate of subtotal
            if (shippingByWeightRecord.PercentageRateOfSubtotal > decimal.Zero)
            {
                shippingTotal += Math.Round((decimal)((((float)subTotal) * ((float)shippingByWeightRecord.PercentageRateOfSubtotal)) / 100f), 2);
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

            if (getShippingOptionRequest.Items == null || !getShippingOptionRequest.Items.Any())
            {
                response.AddError("No shipment items");
                return response;
            }

            //choose the shipping rate calculation method
            if (_fixedOrByWeightSettings.ShippingByWeightEnabled)
            {
                //shipping rate calculation by products weight

                if (getShippingOptionRequest.ShippingAddress == null)
                {
                    response.AddError("Shipping address is not set");
                    return response;
                }

                var storeId = getShippingOptionRequest.StoreId;

                if (storeId == 0)
                    storeId = _storeContext.CurrentStore.Id;

                var countryId = getShippingOptionRequest.ShippingAddress.CountryId.HasValue ? getShippingOptionRequest.ShippingAddress.CountryId.Value : 0;
                var stateProvinceId = getShippingOptionRequest.ShippingAddress.StateProvinceId.HasValue ? getShippingOptionRequest.ShippingAddress.StateProvinceId.Value : 0;
                var warehouseId = getShippingOptionRequest.WarehouseFrom != null ? getShippingOptionRequest.WarehouseFrom.Id : 0;
                var zip = getShippingOptionRequest.ShippingAddress.ZipPostalCode;
                var subTotal = decimal.Zero;

                foreach (var packageItem in getShippingOptionRequest.Items)
                {
                    if (packageItem.ShoppingCartItem.IsFreeShipping)
                        continue;
                    //TODO we should use getShippingOptionRequest.Items.GetQuantity() method to get subtotal
                    subTotal += _priceCalculationService.GetSubTotal(packageItem.ShoppingCartItem);
                }

                var weight = _shippingService.GetTotalWeight(getShippingOptionRequest);

                var shippingMethods = _shippingService.GetAllShippingMethods(countryId);
                foreach (var shippingMethod in shippingMethods)
                {
                    var rate = GetRate(subTotal, weight, shippingMethod.Id, storeId, warehouseId, countryId, stateProvinceId, zip);

                    if (!rate.HasValue) continue;

                    var shippingOption = new ShippingOption
                    {
                        Name = shippingMethod.GetLocalized(x => x.Name),
                        Description = shippingMethod.GetLocalized(x => x.Description),
                        Rate = rate.Value
                    };

                    response.ShippingOptions.Add(shippingOption);
                }
            }
            else
            {
                //shipping rate calculation by fixed rate

                var restrictByCountryId = getShippingOptionRequest.ShippingAddress != null && getShippingOptionRequest.ShippingAddress.Country != null ? (int?)getShippingOptionRequest.ShippingAddress.Country.Id : null;
                var shippingMethods = _shippingService.GetAllShippingMethods(restrictByCountryId);

                foreach (var shippingMethod in shippingMethods)
                {
                    var shippingOption = new ShippingOption
                    {
                        Name = shippingMethod.GetLocalized(x => x.Name),
                        Description = shippingMethod.GetLocalized(x => x.Description),
                        Rate = GetRate(shippingMethod.Id)
                    };
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
            //if the "shipping calculation by weight" method is selected, the fixed rate isn't calculated
            if (_fixedOrByWeightSettings.ShippingByWeightEnabled)
                return null;

            if (getShippingOptionRequest == null)
                throw new ArgumentNullException("getShippingOptionRequest");

            var restrictByCountryId = getShippingOptionRequest.ShippingAddress != null && getShippingOptionRequest.ShippingAddress.Country != null ? (int?)getShippingOptionRequest.ShippingAddress.Country.Id : null;
            var shippingMethods = _shippingService.GetAllShippingMethods(restrictByCountryId);
            
            var rates = new List<decimal>();
            foreach (var shippingMethod in shippingMethods)
            {
                var rate = GetRate(shippingMethod.Id);
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
            controllerName = "FixedOrByWeight";
            routeValues = new RouteValueDictionary { { "Namespaces", "Nop.Plugin.Shipping.FixedOrByWeight.Controllers" }, { "area", null } };
        }

        /// <summary>
        /// Install plugin
        /// </summary>
        public override void Install()
        {
            //settings
            var settings = new FixedOrByWeightSettings
            {
                LimitMethodsToCreated = false,
                ShippingByWeightEnabled = false
            };
            _settingService.SaveSetting(settings);

            //database objects
            _objectContext.Install();

            //locales
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.FixedOrByWeight.ShippingByWeight", "By Weight");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.FixedOrByWeight.Fixed", "Fixed Rate");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.FixedOrByWeight.Fields.Rate", "Rate");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.FixedOrByWeight.Fields.Store", "Store");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.FixedOrByWeight.Fields.Store.Hint", "If an asterisk is selected, then this shipping rate will apply to all stores.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.FixedOrByWeight.Fields.Warehouse", "Warehouse");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.FixedOrByWeight.Fields.Warehouse.Hint", "If an asterisk is selected, then this shipping rate will apply to all warehouses.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.FixedOrByWeight.Fields.Country", "Country");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.FixedOrByWeight.Fields.Country.Hint", "If an asterisk is selected, then this shipping rate will apply to all customers, regardless of the country.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.FixedOrByWeight.Fields.StateProvince", "State / province");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.FixedOrByWeight.Fields.StateProvince.Hint", "If an asterisk is selected, then this shipping rate will apply to all customers from the given country, regardless of the state.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.FixedOrByWeight.Fields.Zip", "Zip");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.FixedOrByWeight.Fields.Zip.Hint", "Zip / postal code. If zip is empty, then this shipping rate will apply to all customers from the given country or state, regardless of the zip code.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.FixedOrByWeight.Fields.ShippingMethod", "Shipping method");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.FixedOrByWeight.Fields.ShippingMethod.Hint", "Choose shipping method");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.FixedOrByWeight.Fields.From", "Order weight from");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.FixedOrByWeight.Fields.From.Hint", "Order weight from.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.FixedOrByWeight.Fields.To", "Order weight to");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.FixedOrByWeight.Fields.To.Hint", "Order weight to.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.FixedOrByWeight.Fields.AdditionalFixedCost", "Additional fixed cost");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.FixedOrByWeight.Fields.AdditionalFixedCost.Hint", "Specify an additional fixed cost per shopping cart for this option. Set to 0 if you don't want an additional fixed cost to be applied.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.FixedOrByWeight.Fields.LowerWeightLimit", "Lower weight limit");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.FixedOrByWeight.Fields.LowerWeightLimit.Hint", "Lower weight limit. This field can be used for \"per extra weight unit\" scenarios.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.FixedOrByWeight.Fields.PercentageRateOfSubtotal", "Charge percentage (of subtotal)");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.FixedOrByWeight.Fields.PercentageRateOfSubtotal.Hint", "Charge percentage (of subtotal).");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.FixedOrByWeight.Fields.RatePerWeightUnit", "Rate per weight unit");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.FixedOrByWeight.Fields.RatePerWeightUnit.Hint", "Rate per weight unit.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.FixedOrByWeight.Fields.LimitMethodsToCreated", "Limit shipping methods to configured ones");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.FixedOrByWeight.Fields.LimitMethodsToCreated.Hint", "If you check this option, then your customers will be limited to shipping options configured here. Otherwise, they'll be able to choose any existing shipping options even they've not configured here (zero shipping fee in this case).");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.FixedOrByWeight.Fields.DataHtml", "Data");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.FixedOrByWeight.AddRecord", "Add record");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.FixedOrByWeight.Formula", "Formula to calculate rates");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.FixedOrByWeight.Formula.Value", "[additional fixed cost] + ([order total weight] - [lower weight limit]) * [rate per weight unit] + [order subtotal] * [charge percentage]");
            
            base.Install();
        }
        
        /// <summary>
        /// Uninstall plugin
        /// </summary>
        public override void Uninstall()
        {
            //settings
            _settingService.DeleteSetting<FixedOrByWeightSettings>();

            //database objects
            _objectContext.Uninstall();

            //locales
            this.DeletePluginLocaleResource("Plugins.Shipping.FixedOrByWeight.Fields.Rate");
            this.DeletePluginLocaleResource("Plugins.Shipping.FixedOrByWeight.Fields.Store");
            this.DeletePluginLocaleResource("Plugins.Shipping.FixedOrByWeight.Fields.Store.Hint");
            this.DeletePluginLocaleResource("Plugins.Shipping.FixedOrByWeight.Fields.Warehouse");
            this.DeletePluginLocaleResource("Plugins.Shipping.FixedOrByWeight.Fields.Warehouse.Hint");
            this.DeletePluginLocaleResource("Plugins.Shipping.FixedOrByWeight.Fields.Country");
            this.DeletePluginLocaleResource("Plugins.Shipping.FixedOrByWeight.Fields.Country.Hint");
            this.DeletePluginLocaleResource("Plugins.Shipping.FixedOrByWeight.Fields.StateProvince");
            this.DeletePluginLocaleResource("Plugins.Shipping.FixedOrByWeight.Fields.StateProvince.Hint");
            this.DeletePluginLocaleResource("Plugins.Shipping.FixedOrByWeight.Fields.Zip");
            this.DeletePluginLocaleResource("Plugins.Shipping.FixedOrByWeight.Fields.Zip.Hint");
            this.DeletePluginLocaleResource("Plugins.Shipping.FixedOrByWeight.Fields.ShippingMethod");
            this.DeletePluginLocaleResource("Plugins.Shipping.FixedOrByWeight.Fields.ShippingMethod.Hint");
            this.DeletePluginLocaleResource("Plugins.Shipping.FixedOrByWeight.Fields.From");
            this.DeletePluginLocaleResource("Plugins.Shipping.FixedOrByWeight.Fields.From.Hint");
            this.DeletePluginLocaleResource("Plugins.Shipping.FixedOrByWeight.Fields.To");
            this.DeletePluginLocaleResource("Plugins.Shipping.FixedOrByWeight.Fields.To.Hint");
            this.DeletePluginLocaleResource("Plugins.Shipping.FixedOrByWeight.Fields.AdditionalFixedCost");
            this.DeletePluginLocaleResource("Plugins.Shipping.FixedOrByWeight.Fields.AdditionalFixedCost.Hint");
            this.DeletePluginLocaleResource("Plugins.Shipping.FixedOrByWeight.Fields.LowerWeightLimit");
            this.DeletePluginLocaleResource("Plugins.Shipping.FixedOrByWeight.Fields.LowerWeightLimit.Hint");
            this.DeletePluginLocaleResource("Plugins.Shipping.FixedOrByWeight.Fields.PercentageRateOfSubtotal");
            this.DeletePluginLocaleResource("Plugins.Shipping.FixedOrByWeight.Fields.PercentageRateOfSubtotal.Hint");
            this.DeletePluginLocaleResource("Plugins.Shipping.FixedOrByWeight.Fields.RatePerWeightUnit");
            this.DeletePluginLocaleResource("Plugins.Shipping.FixedOrByWeight.Fields.RatePerWeightUnit.Hint");
            this.DeletePluginLocaleResource("Plugins.Shipping.FixedOrByWeight.Fields.LimitMethodsToCreated");
            this.DeletePluginLocaleResource("Plugins.Shipping.FixedOrByWeight.Fields.LimitMethodsToCreated.Hint");
            this.DeletePluginLocaleResource("Plugins.Shipping.FixedOrByWeight.Fields.DataHtml");
            this.DeletePluginLocaleResource("Plugins.Shipping.FixedOrByWeight.AddRecord");
            this.DeletePluginLocaleResource("Plugins.Shipping.FixedOrByWeight.Formula");
            this.DeletePluginLocaleResource("Plugins.Shipping.FixedOrByWeight.Formula.Value");

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
