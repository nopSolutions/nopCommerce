using System;
using System.Linq;
using System.Web.Routing;
using Nop.Core;
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
        private readonly IStoreContext _storeContext;
        private readonly IShippingByWeightService _shippingByWeightService;
        private readonly IPriceCalculationService _priceCalculationService;
        private readonly ShippingByWeightSettings _shippingByWeightSettings;
        private readonly ShippingByWeightObjectContext _objectContext;
        private readonly ISettingService _settingService;

        #endregion

        #region Ctor
        public ByWeightShippingComputationMethod(IShippingService shippingService,
            IStoreContext storeContext,
            IShippingByWeightService shippingByWeightService,
            IPriceCalculationService priceCalculationService, 
            ShippingByWeightSettings shippingByWeightSettings,
            ShippingByWeightObjectContext objectContext,
            ISettingService settingService)
        {
            this._shippingService = shippingService;
            this._storeContext = storeContext;
            this._shippingByWeightService = shippingByWeightService;
            this._priceCalculationService = priceCalculationService;
            this._shippingByWeightSettings = shippingByWeightSettings;
            this._objectContext = objectContext;
            this._settingService = settingService;
        }
        #endregion

        #region Utilities
        
        private decimal? GetRate(decimal subTotal, decimal weight, int shippingMethodId,
            int storeId, int warehouseId, int countryId, int stateProvinceId, string zip)
        {
            var shippingByWeightRecord = _shippingByWeightService.FindRecord(shippingMethodId,
                storeId, warehouseId, countryId, stateProvinceId, zip, weight);
            if (shippingByWeightRecord == null)
            {
                if (_shippingByWeightSettings.LimitMethodsToCreated)
                    return null;
                
                return decimal.Zero;
            }

            //additional fixed cost
            decimal shippingTotal = shippingByWeightRecord.AdditionalFixedCost;
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
            if (getShippingOptionRequest.ShippingAddress == null)
            {
                response.AddError("Shipping address is not set");
                return response;
            }

            var storeId = getShippingOptionRequest.StoreId;
            if (storeId == 0)
                storeId = _storeContext.CurrentStore.Id;
            int countryId = getShippingOptionRequest.ShippingAddress.CountryId.HasValue ? getShippingOptionRequest.ShippingAddress.CountryId.Value : 0;
            int stateProvinceId = getShippingOptionRequest.ShippingAddress.StateProvinceId.HasValue ? getShippingOptionRequest.ShippingAddress.StateProvinceId.Value : 0;
            int warehouseId = getShippingOptionRequest.WarehouseFrom != null ? getShippingOptionRequest.WarehouseFrom.Id : 0;
            string zip = getShippingOptionRequest.ShippingAddress.ZipPostalCode;
            decimal subTotal = decimal.Zero;
            foreach (var packageItem in getShippingOptionRequest.Items)
            {
                if (packageItem.ShoppingCartItem.IsFreeShipping)
                    continue;
                //TODO we should use getShippingOptionRequest.Items.GetQuantity() method to get subtotal
                subTotal += _priceCalculationService.GetSubTotal(packageItem.ShoppingCartItem);
            }
            decimal weight = _shippingService.GetTotalWeight(getShippingOptionRequest);

            var shippingMethods = _shippingService.GetAllShippingMethods(countryId);
            foreach (var shippingMethod in shippingMethods)
            {
                decimal? rate = GetRate(subTotal, weight, shippingMethod.Id,
                    storeId, warehouseId, countryId, stateProvinceId, zip);
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
            routeValues = new RouteValueDictionary { { "Namespaces", "Nop.Plugin.Shipping.ByWeight.Controllers" }, { "area", null } };
        }
        
        /// <summary>
        /// Install plugin
        /// </summary>
        public override void Install()
        {
            //settings
            var settings = new ShippingByWeightSettings
            {
                LimitMethodsToCreated = false,
            };
            _settingService.SaveSetting(settings);


            //database objects
            _objectContext.Install();

            //locales
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.ByWeight.Fields.Store", "Store");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.ByWeight.Fields.Store.Hint", "If an asterisk is selected, then this shipping rate will apply to all stores.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.ByWeight.Fields.Warehouse", "Warehouse");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.ByWeight.Fields.Warehouse.Hint", "If an asterisk is selected, then this shipping rate will apply to all warehouses.");
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
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.ByWeight.Fields.To.Hint", "Order weight to.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.ByWeight.Fields.AdditionalFixedCost", "Additional fixed cost");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.ByWeight.Fields.AdditionalFixedCost.Hint", "Specify an additional fixed cost per shopping cart for this option. Set to 0 if you don't want an additional fixed cost to be applied.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.ByWeight.Fields.LowerWeightLimit", "Lower weight limit");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.ByWeight.Fields.LowerWeightLimit.Hint", "Lower weight limit. This field can be used for \"per extra weight unit\" scenarios.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.ByWeight.Fields.PercentageRateOfSubtotal", "Charge percentage (of subtotal)");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.ByWeight.Fields.PercentageRateOfSubtotal.Hint", "Charge percentage (of subtotal).");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.ByWeight.Fields.RatePerWeightUnit", "Rate per weight unit");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.ByWeight.Fields.RatePerWeightUnit.Hint", "Rate per weight unit.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.ByWeight.Fields.LimitMethodsToCreated", "Limit shipping methods to configured ones");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.ByWeight.Fields.LimitMethodsToCreated.Hint", "If you check this option, then your customers will be limited to shipping options configured here. Otherwise, they'll be able to choose any existing shipping options even they've not configured here (zero shipping fee in this case).");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.ByWeight.Fields.DataHtml", "Data");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.ByWeight.AddRecord", "Add record");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.ByWeight.Formula", "Formula to calculate rates");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.ByWeight.Formula.Value", "[additional fixed cost] + ([order total weight] - [lower weight limit]) * [rate per weight unit] + [order subtotal] * [charge percentage]");
            
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
            this.DeletePluginLocaleResource("Plugins.Shipping.ByWeight.Fields.Store");
            this.DeletePluginLocaleResource("Plugins.Shipping.ByWeight.Fields.Store.Hint");
            this.DeletePluginLocaleResource("Plugins.Shipping.ByWeight.Fields.Warehouse");
            this.DeletePluginLocaleResource("Plugins.Shipping.ByWeight.Fields.Warehouse.Hint");
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
            this.DeletePluginLocaleResource("Plugins.Shipping.ByWeight.Fields.AdditionalFixedCost");
            this.DeletePluginLocaleResource("Plugins.Shipping.ByWeight.Fields.AdditionalFixedCost.Hint");
            this.DeletePluginLocaleResource("Plugins.Shipping.ByWeight.Fields.LowerWeightLimit");
            this.DeletePluginLocaleResource("Plugins.Shipping.ByWeight.Fields.LowerWeightLimit.Hint");
            this.DeletePluginLocaleResource("Plugins.Shipping.ByWeight.Fields.PercentageRateOfSubtotal");
            this.DeletePluginLocaleResource("Plugins.Shipping.ByWeight.Fields.PercentageRateOfSubtotal.Hint");
            this.DeletePluginLocaleResource("Plugins.Shipping.ByWeight.Fields.RatePerWeightUnit");
            this.DeletePluginLocaleResource("Plugins.Shipping.ByWeight.Fields.RatePerWeightUnit.Hint");
            this.DeletePluginLocaleResource("Plugins.Shipping.ByWeight.Fields.LimitMethodsToCreated");
            this.DeletePluginLocaleResource("Plugins.Shipping.ByWeight.Fields.LimitMethodsToCreated.Hint");
            this.DeletePluginLocaleResource("Plugins.Shipping.ByWeight.Fields.DataHtml");
            this.DeletePluginLocaleResource("Plugins.Shipping.ByWeight.AddRecord");
            this.DeletePluginLocaleResource("Plugins.Shipping.ByWeight.Formula");
            this.DeletePluginLocaleResource("Plugins.Shipping.ByWeight.Formula.Value");
            
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
