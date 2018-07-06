using System;
using System.Linq;
using Nop.Core;
using Nop.Core.Domain.Shipping;
using Nop.Core.Plugins;
using Nop.Plugin.Shipping.FixedByWeightByTotal.Data;
using Nop.Plugin.Shipping.FixedByWeightByTotal.Services;
using Nop.Services.Catalog;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Shipping;
using Nop.Services.Shipping.Tracking;

namespace Nop.Plugin.Shipping.FixedByWeightByTotal
{
    /// <summary>
    /// Fixed rate or by weight shipping computation method 
    /// </summary>
    public class FixedByWeightByTotalComputationMethod : BasePlugin, IShippingRateComputationMethod
    {
        #region Fields

        private readonly FixedByWeightByTotalSettings _fixedByWeightByTotalSettings;
        private readonly ILocalizationService _localizationService;
        private readonly IPriceCalculationService _priceCalculationService;
        private readonly ISettingService _settingService;
        private readonly IShippingByWeightByTotalService _shippingByWeightByTotalService;
        private readonly IShippingService _shippingService;
        private readonly IStoreContext _storeContext;
        private readonly IWebHelper _webHelper;
        private readonly ShippingByWeightByTotalObjectContext _objectContext;

        #endregion

        #region Ctor

        public FixedByWeightByTotalComputationMethod(FixedByWeightByTotalSettings fixedByWeightByTotalSettings,
            ILocalizationService localizationService,
            IPriceCalculationService priceCalculationService,
            ISettingService settingService,
            IShippingByWeightByTotalService shippingByWeightByTotalService,
            IShippingService shippingService,
            IStoreContext storeContext,
            IWebHelper webHelper,
            ShippingByWeightByTotalObjectContext objectContext)
        {
            this._fixedByWeightByTotalSettings = fixedByWeightByTotalSettings;
            this._localizationService = localizationService;
            this._priceCalculationService = priceCalculationService;
            this._settingService = settingService;
            this._shippingByWeightByTotalService = shippingByWeightByTotalService;
            this._shippingService = shippingService;
            this._storeContext = storeContext;
            this._webHelper = webHelper;
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
            return _settingService.GetSettingByKey<decimal>(string.Format(FixedByWeightByTotalDefaults.FixedRateSettingsKey, shippingMethodId));
        }

        /// <summary>
        /// Get rate by weight and by total
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
            var shippingByWeightByTotalRecord = _shippingByWeightByTotalService.FindRecords(shippingMethodId, storeId, warehouseId, countryId, stateProvinceId, zip, weight, subTotal);
            if (shippingByWeightByTotalRecord == null)
            {
                if (_fixedByWeightByTotalSettings.LimitMethodsToCreated)
                    return null;

                return decimal.Zero;
            }

            //additional fixed cost
            var shippingTotal = shippingByWeightByTotalRecord.AdditionalFixedCost;

            //charge amount per weight unit
            if (shippingByWeightByTotalRecord.RatePerWeightUnit > decimal.Zero)
            {
                var weightRate = Math.Max(weight - shippingByWeightByTotalRecord.LowerWeightLimit, decimal.Zero);
                shippingTotal += shippingByWeightByTotalRecord.RatePerWeightUnit * weightRate;
            }

            //percentage rate of subtotal
            if (shippingByWeightByTotalRecord.PercentageRateOfSubtotal > decimal.Zero)
            {
                shippingTotal += Math.Round((decimal)((((float)subTotal) * ((float)shippingByWeightByTotalRecord.PercentageRateOfSubtotal)) / 100f), 2);
            }

            return Math.Max(shippingTotal, decimal.Zero);
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

            var response = new GetShippingOptionResponse();

            if (getShippingOptionRequest.Items == null || !getShippingOptionRequest.Items.Any())
            {
                response.AddError("No shipment items");
                return response;
            }

            //choose the shipping rate calculation method
            if (_fixedByWeightByTotalSettings.ShippingByWeightByTotalEnabled)
            {
                //shipping rate calculation by products weight

                if (getShippingOptionRequest.ShippingAddress == null)
                {
                    response.AddError("Shipping address is not set");
                    return response;
                }

                var storeId = getShippingOptionRequest.StoreId != 0 ? getShippingOptionRequest.StoreId : _storeContext.CurrentStore.Id;
                var countryId = getShippingOptionRequest.ShippingAddress.CountryId ?? 0;
                var stateProvinceId = getShippingOptionRequest.ShippingAddress.StateProvinceId ?? 0;
                var warehouseId = getShippingOptionRequest.WarehouseFrom?.Id ?? 0;
                var zip = getShippingOptionRequest.ShippingAddress.ZipPostalCode;

                //get subtotal of shipped items
                var subTotal = decimal.Zero;
                foreach (var packageItem in getShippingOptionRequest.Items)
                {
                    if (_shippingService.IsFreeShipping(packageItem.ShoppingCartItem))
                        continue;

                    //TODO we should use getShippingOptionRequest.Items.GetQuantity() method to get subtotal
                    subTotal += _priceCalculationService.GetSubTotal(packageItem.ShoppingCartItem);
                }

                //get weight of shipped items (excluding items with free shipping)
                var weight = _shippingService.GetTotalWeight(getShippingOptionRequest, ignoreFreeShippedItems: true);

                foreach (var shippingMethod in _shippingService.GetAllShippingMethods(countryId))
                {
                    var rate = GetRate(subTotal, weight, shippingMethod.Id, storeId, warehouseId, countryId, stateProvinceId, zip);
                    if (!rate.HasValue)
                        continue;

                    response.ShippingOptions.Add(new ShippingOption
                    {
                        Name = _localizationService.GetLocalized(shippingMethod, x => x.Name),
                        Description = _localizationService.GetLocalized(shippingMethod, x => x.Description),
                        Rate = rate.Value
                    });
                }
            }
            else
            {
                //shipping rate calculation by fixed rate
                var restrictByCountryId = getShippingOptionRequest.ShippingAddress?.Country?.Id;
                response.ShippingOptions = _shippingService.GetAllShippingMethods(restrictByCountryId).Select(shippingMethod => new ShippingOption
                {
                    Name = _localizationService.GetLocalized(shippingMethod, x => x.Name),
                    Description = _localizationService.GetLocalized(shippingMethod, x => x.Description),
                    Rate = GetRate(shippingMethod.Id)
                }).ToList();
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

            //if the "shipping calculation by weight" method is selected, the fixed rate isn't calculated
            if (_fixedByWeightByTotalSettings.ShippingByWeightByTotalEnabled)
                return null;

            var restrictByCountryId = getShippingOptionRequest.ShippingAddress?.Country?.Id;
            var rates = _shippingService.GetAllShippingMethods(restrictByCountryId)
                .Select(shippingMethod => GetRate(shippingMethod.Id)).Distinct().ToList();

            //return default rate if all of them equal
            if (rates.Count == 1)
                return rates.FirstOrDefault();

            return null;
        }

        /// <summary>
        /// Gets a configuration page URL
        /// </summary>
        public override string GetConfigurationPageUrl()
        {
            return $"{_webHelper.GetStoreLocation()}Admin/FixedByWeightByTotal/Configure";
        }

        /// <summary>
        /// Install plugin
        /// </summary>
        public override void Install()
        {
            //settings
            _settingService.SaveSetting(new FixedByWeightByTotalSettings());

            //database objects
            _objectContext.Install();

            //locales
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Shipping.FixedByWeightByTotal.AddRecord", "Add record");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Shipping.FixedByWeightByTotal.Fields.AdditionalFixedCost", "Additional fixed cost");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Shipping.FixedByWeightByTotal.Fields.AdditionalFixedCost.Hint", "Specify an additional fixed cost per shopping cart for this option. Set to 0 if you don't want an additional fixed cost to be applied.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Shipping.FixedByWeightByTotal.Fields.Country", "Country");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Shipping.FixedByWeightByTotal.Fields.Country.Hint", "If an asterisk is selected, then this shipping rate will apply to all customers, regardless of the country.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Shipping.FixedByWeightByTotal.Fields.DataHtml", "Data");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Shipping.FixedByWeightByTotal.Fields.LimitMethodsToCreated", "Limit shipping methods to configured ones");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Shipping.FixedByWeightByTotal.Fields.LimitMethodsToCreated.Hint", "If you check this option, then your customers will be limited to shipping options configured here. Otherwise, they'll be able to choose any existing shipping options even they are not configured here (zero shipping fee in this case).");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Shipping.FixedByWeightByTotal.Fields.LowerWeightLimit", "Lower weight limit");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Shipping.FixedByWeightByTotal.Fields.LowerWeightLimit.Hint", "Lower weight limit. This field can be used for \"per extra weight unit\" scenarios.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Shipping.FixedByWeightByTotal.Fields.OrderSubtotalFrom", "Order subtotal from");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Shipping.FixedByWeightByTotal.Fields.OrderSubtotalFrom.Hint", "Order subtotal from.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Shipping.FixedByWeightByTotal.Fields.OrderSubtotalTo", "Order subtotal to");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Shipping.FixedByWeightByTotal.Fields.OrderSubtotalTo.Hint", "Order subtotal to.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Shipping.FixedByWeightByTotal.Fields.PercentageRateOfSubtotal", "Charge percentage (of subtotal)");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Shipping.FixedByWeightByTotal.Fields.PercentageRateOfSubtotal.Hint", "Charge percentage (of subtotal).");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Shipping.FixedByWeightByTotal.Fields.Rate", "Rate");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Shipping.FixedByWeightByTotal.Fields.RatePerWeightUnit", "Rate per weight unit");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Shipping.FixedByWeightByTotal.Fields.RatePerWeightUnit.Hint", "Rate per weight unit.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Shipping.FixedByWeightByTotal.Fields.ShippingMethod", "Shipping method");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Shipping.FixedByWeightByTotal.Fields.ShippingMethod.Hint", "Choose shipping method.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Shipping.FixedByWeightByTotal.Fields.StateProvince", "State / province");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Shipping.FixedByWeightByTotal.Fields.StateProvince.Hint", "If an asterisk is selected, then this shipping rate will apply to all customers from the given country, regardless of the state.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Shipping.FixedByWeightByTotal.Fields.Store", "Store");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Shipping.FixedByWeightByTotal.Fields.Store.Hint", "If an asterisk is selected, then this shipping rate will apply to all stores.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Shipping.FixedByWeightByTotal.Fields.Warehouse", "Warehouse");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Shipping.FixedByWeightByTotal.Fields.Warehouse.Hint", "If an asterisk is selected, then this shipping rate will apply to all warehouses.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Shipping.FixedByWeightByTotal.Fields.WeightFrom", "Order weight from");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Shipping.FixedByWeightByTotal.Fields.WeightFrom.Hint", "Order weight from.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Shipping.FixedByWeightByTotal.Fields.WeightTo", "Order weight to");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Shipping.FixedByWeightByTotal.Fields.WeightTo.Hint", "Order weight to.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Shipping.FixedByWeightByTotal.Fields.Zip", "Zip");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Shipping.FixedByWeightByTotal.Fields.Zip.Hint", "Zip / postal code. If zip is empty, then this shipping rate will apply to all customers from the given country or state, regardless of the zip code.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Shipping.FixedByWeightByTotal.Fixed", "Fixed Rate");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Shipping.FixedByWeightByTotal.Formula", "Formula to calculate rates");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Shipping.FixedByWeightByTotal.Formula.Value", "[additional fixed cost] + ([order total weight] - [lower weight limit]) * [rate per weight unit] + [order subtotal] * [charge percentage]");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Shipping.FixedByWeightByTotal.ShippingByWeight", "By Weight");

            base.Install();
        }

        /// <summary>
        /// Uninstall plugin
        /// </summary>
        public override void Uninstall()
        {
            //settings
            _settingService.DeleteSetting<FixedByWeightByTotalSettings>();

            //fixed rates
            var fixedRates = _shippingService.GetAllShippingMethods()
                .Select(shippingMethod => _settingService.GetSetting(
                    string.Format(FixedByWeightByTotalDefaults.FixedRateSettingsKey, shippingMethod.Id)))
                .Where(setting => setting != null).ToList();
            _settingService.DeleteSettings(fixedRates);

            //database objects
            _objectContext.Uninstall();

            //locales
            _localizationService.DeletePluginLocaleResource("Plugins.Shipping.FixedByWeightByTotal.AddRecord");
            _localizationService.DeletePluginLocaleResource("Plugins.Shipping.FixedByWeightByTotal.Fields.AdditionalFixedCost");
            _localizationService.DeletePluginLocaleResource("Plugins.Shipping.FixedByWeightByTotal.Fields.AdditionalFixedCost.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Shipping.FixedByWeightByTotal.Fields.Country");
            _localizationService.DeletePluginLocaleResource("Plugins.Shipping.FixedByWeightByTotal.Fields.Country.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Shipping.FixedByWeightByTotal.Fields.DataHtml");
            _localizationService.DeletePluginLocaleResource("Plugins.Shipping.FixedByWeightByTotal.Fields.LimitMethodsToCreated");
            _localizationService.DeletePluginLocaleResource("Plugins.Shipping.FixedByWeightByTotal.Fields.LimitMethodsToCreated.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Shipping.FixedByWeightByTotal.Fields.LowerWeightLimit");
            _localizationService.DeletePluginLocaleResource("Plugins.Shipping.FixedByWeightByTotal.Fields.LowerWeightLimit.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Shipping.FixedByWeightByTotal.Fields.OrderSubtotalFrom");
            _localizationService.DeletePluginLocaleResource("Plugins.Shipping.FixedByWeightByTotal.Fields.OrderSubtotalFrom.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Shipping.FixedByWeightByTotal.Fields.OrderSubtotalTo");
            _localizationService.DeletePluginLocaleResource("Plugins.Shipping.FixedByWeightByTotal.Fields.OrderSubtotalTo.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Shipping.FixedByWeightByTotal.Fields.PercentageRateOfSubtotal");
            _localizationService.DeletePluginLocaleResource("Plugins.Shipping.FixedByWeightByTotal.Fields.PercentageRateOfSubtotal.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Shipping.FixedByWeightByTotal.Fields.Rate");
            _localizationService.DeletePluginLocaleResource("Plugins.Shipping.FixedByWeightByTotal.Fields.RatePerWeightUnit");
            _localizationService.DeletePluginLocaleResource("Plugins.Shipping.FixedByWeightByTotal.Fields.RatePerWeightUnit.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Shipping.FixedByWeightByTotal.Fields.ShippingMethod");
            _localizationService.DeletePluginLocaleResource("Plugins.Shipping.FixedByWeightByTotal.Fields.ShippingMethod.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Shipping.FixedByWeightByTotal.Fields.StateProvince");
            _localizationService.DeletePluginLocaleResource("Plugins.Shipping.FixedByWeightByTotal.Fields.StateProvince.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Shipping.FixedByWeightByTotal.Fields.Store");
            _localizationService.DeletePluginLocaleResource("Plugins.Shipping.FixedByWeightByTotal.Fields.Store.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Shipping.FixedByWeightByTotal.Fields.Warehouse");
            _localizationService.DeletePluginLocaleResource("Plugins.Shipping.FixedByWeightByTotal.Fields.Warehouse.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Shipping.FixedByWeightByTotal.Fields.WeightFrom");
            _localizationService.DeletePluginLocaleResource("Plugins.Shipping.FixedByWeightByTotal.Fields.WeightFrom.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Shipping.FixedByWeightByTotal.Fields.WeightTo");
            _localizationService.DeletePluginLocaleResource("Plugins.Shipping.FixedByWeightByTotal.Fields.WeightTo.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Shipping.FixedByWeightByTotal.Fields.Zip");
            _localizationService.DeletePluginLocaleResource("Plugins.Shipping.FixedByWeightByTotal.Fields.Zip.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Shipping.FixedByWeightByTotal.Fixed");
            _localizationService.DeletePluginLocaleResource("Plugins.Shipping.FixedByWeightByTotal.Formula");
            _localizationService.DeletePluginLocaleResource("Plugins.Shipping.FixedByWeightByTotal.Formula.Value");
            _localizationService.DeletePluginLocaleResource("Plugins.Shipping.FixedByWeightByTotal.ShippingByWeight");

            base.Uninstall();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a shipping rate computation method type
        /// </summary>
        public ShippingRateComputationMethodType ShippingRateComputationMethodType
        {
            get { return ShippingRateComputationMethodType.Offline; }
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