using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Domain.Shipping;
using Nop.Plugin.Shipping.FixedByWeightByTotal.Domain;
using Nop.Plugin.Shipping.FixedByWeightByTotal.Services;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Orders;
using Nop.Services.Plugins;
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
        private readonly IShoppingCartService _shoppingCartService;
        private readonly ISettingService _settingService;
        private readonly IShippingByWeightByTotalService _shippingByWeightByTotalService;
        private readonly IShippingService _shippingService;
        private readonly IStoreContext _storeContext;
        private readonly IWebHelper _webHelper;

        #endregion

        #region Ctor

        public FixedByWeightByTotalComputationMethod(FixedByWeightByTotalSettings fixedByWeightByTotalSettings,
            ILocalizationService localizationService,
            IShoppingCartService shoppingCartService,
            ISettingService settingService,
            IShippingByWeightByTotalService shippingByWeightByTotalService,
            IShippingService shippingService,
            IStoreContext storeContext,
            IWebHelper webHelper)
        {
            _fixedByWeightByTotalSettings = fixedByWeightByTotalSettings;
            _localizationService = localizationService;
            _shoppingCartService = shoppingCartService;
            _settingService = settingService;
            _shippingByWeightByTotalService = shippingByWeightByTotalService;
            _shippingService = shippingService;
            _storeContext = storeContext;
            _webHelper = webHelper;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Get fixed rate
        /// </summary>
        /// <param name="shippingMethodId">Shipping method ID</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the rate
        /// </returns>
        private async Task<decimal> GetRateAsync(int shippingMethodId)
        {
            return await _settingService.GetSettingByKeyAsync<decimal>(string.Format(FixedByWeightByTotalDefaults.FixedRateSettingsKey, shippingMethodId));
        }

        /// <summary>
        /// Gets the transit days
        /// </summary>
        /// <param name="shippingMethodId">Shipping method ID</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the ransit days
        /// </returns>
        private async Task<int?> GetTransitDaysAsync(int shippingMethodId)
        {
            return await _settingService.GetSettingByKeyAsync<int?>(string.Format(FixedByWeightByTotalDefaults.TransitDaysSettingsKey, shippingMethodId));
        }

        /// <summary>
        /// Get rate by weight and by total
        /// </summary>
        /// <param name="shippingByWeightByTotalRecord">Shipping by weight/by total record</param>
        /// <param name="subTotal">Subtotal</param>
        /// <param name="weight">Weight</param>
        /// <returns>Rate</returns>
        private decimal GetRate(ShippingByWeightByTotalRecord shippingByWeightByTotalRecord, decimal subTotal, decimal weight)
        {
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
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the represents a response of getting shipping rate options
        /// </returns>
        public async Task<GetShippingOptionResponse> GetShippingOptionsAsync(GetShippingOptionRequest getShippingOptionRequest)
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

                var store = await _storeContext.GetCurrentStoreAsync();
                var storeId = getShippingOptionRequest.StoreId != 0 ? getShippingOptionRequest.StoreId : store.Id;
                var countryId = getShippingOptionRequest.ShippingAddress.CountryId ?? 0;
                var stateProvinceId = getShippingOptionRequest.ShippingAddress.StateProvinceId ?? 0;
                var warehouseId = getShippingOptionRequest.WarehouseFrom?.Id ?? 0;
                var zip = getShippingOptionRequest.ShippingAddress.ZipPostalCode;

                //get subtotal of shipped items
                var subTotal = decimal.Zero;
                foreach (var packageItem in getShippingOptionRequest.Items)
                {
                    if (await _shippingService.IsFreeShippingAsync(packageItem.ShoppingCartItem))
                        continue;

                    subTotal += (await _shoppingCartService.GetSubTotalAsync(packageItem.ShoppingCartItem, true)).subTotal;
                }

                //get weight of shipped items (excluding items with free shipping)
                var weight = await _shippingService.GetTotalWeightAsync(getShippingOptionRequest, ignoreFreeShippedItems: true);

                foreach (var shippingMethod in await _shippingService.GetAllShippingMethodsAsync(countryId))
                {
                    int? transitDays = null;
                    var rate = decimal.Zero;

                    var shippingByWeightByTotalRecord = await _shippingByWeightByTotalService.FindRecordsAsync(
                            shippingMethod.Id, storeId, warehouseId, countryId, stateProvinceId, zip, weight, subTotal);
                    if (shippingByWeightByTotalRecord == null)
                    {
                        if (_fixedByWeightByTotalSettings.LimitMethodsToCreated)
                            continue;
                    }
                    else
                    {
                        rate = GetRate(shippingByWeightByTotalRecord, subTotal, weight);
                        transitDays = shippingByWeightByTotalRecord.TransitDays;
                    }

                    response.ShippingOptions.Add(new ShippingOption
                    {
                        Name = await _localizationService.GetLocalizedAsync(shippingMethod, x => x.Name),
                        Description = await _localizationService.GetLocalizedAsync(shippingMethod, x => x.Description),
                        Rate = rate,
                        TransitDays = transitDays
                    });
                }
            }
            else
            {
                //shipping rate calculation by fixed rate
                var restrictByCountryId = getShippingOptionRequest.ShippingAddress?.CountryId;
                response.ShippingOptions = await (await _shippingService.GetAllShippingMethodsAsync(restrictByCountryId)).SelectAwait(async shippingMethod => new ShippingOption
                {
                    Name = await _localizationService.GetLocalizedAsync(shippingMethod, x => x.Name),
                    Description = await _localizationService.GetLocalizedAsync(shippingMethod, x => x.Description),
                    Rate = await GetRateAsync(shippingMethod.Id),
                    TransitDays = await GetTransitDaysAsync(shippingMethod.Id)
                }).ToListAsync();
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
        public async Task<decimal?> GetFixedRateAsync(GetShippingOptionRequest getShippingOptionRequest)
        {
            if (getShippingOptionRequest == null)
                throw new ArgumentNullException(nameof(getShippingOptionRequest));

            //if the "shipping calculation by weight" method is selected, the fixed rate isn't calculated
            if (_fixedByWeightByTotalSettings.ShippingByWeightByTotalEnabled)
                return null;

            var restrictByCountryId = getShippingOptionRequest.ShippingAddress?.CountryId;
            var rates = await (await _shippingService.GetAllShippingMethodsAsync(restrictByCountryId))
                .SelectAwait(async shippingMethod => await GetRateAsync(shippingMethod.Id)).Distinct().ToListAsync();

            //return default rate if all of them equal
            if (rates.Count == 1)
                return rates.FirstOrDefault();

            return null;
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
            return Task.FromResult<IShipmentTracker>(null);
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
        /// <returns>A task that represents the asynchronous operation</returns>
        public override async Task InstallAsync()
        {
            //settings
            await _settingService.SaveSettingAsync(new FixedByWeightByTotalSettings());

            //locales
            await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
            {
                ["Plugins.Shipping.FixedByWeightByTotal.AddRecord"] = "Add record",
                ["Plugins.Shipping.FixedByWeightByTotal.Fields.AdditionalFixedCost"] = "Additional fixed cost",
                ["Plugins.Shipping.FixedByWeightByTotal.Fields.AdditionalFixedCost.Hint"] = "Specify an additional fixed cost per shopping cart for this option. Set to 0 if you don't want an additional fixed cost to be applied.",
                ["Plugins.Shipping.FixedByWeightByTotal.Fields.Country"] = "Country",
                ["Plugins.Shipping.FixedByWeightByTotal.Fields.Country.Hint"] = "If an asterisk is selected, then this shipping rate will apply to all customers, regardless of the country.",
                ["Plugins.Shipping.FixedByWeightByTotal.Fields.DataHtml"] = "Data",
                ["Plugins.Shipping.FixedByWeightByTotal.Fields.LimitMethodsToCreated"] = "Limit shipping methods to configured ones",
                ["Plugins.Shipping.FixedByWeightByTotal.Fields.LimitMethodsToCreated.Hint"] = "If you check this option, then your customers will be limited to shipping options configured here. Otherwise, they'll be able to choose any existing shipping options even they are not configured here (zero shipping fee in this case).",
                ["Plugins.Shipping.FixedByWeightByTotal.Fields.LowerWeightLimit"] = "Lower weight limit",
                ["Plugins.Shipping.FixedByWeightByTotal.Fields.LowerWeightLimit.Hint"] = "Lower weight limit. This field can be used for \"per extra weight unit\" scenarios.",
                ["Plugins.Shipping.FixedByWeightByTotal.Fields.OrderSubtotalFrom"] = "Order subtotal from",
                ["Plugins.Shipping.FixedByWeightByTotal.Fields.OrderSubtotalFrom.Hint"] = "Order subtotal from.",
                ["Plugins.Shipping.FixedByWeightByTotal.Fields.OrderSubtotalTo"] = "Order subtotal to",
                ["Plugins.Shipping.FixedByWeightByTotal.Fields.OrderSubtotalTo.Hint"] = "Order subtotal to.",
                ["Plugins.Shipping.FixedByWeightByTotal.Fields.PercentageRateOfSubtotal"] = "Charge percentage (of subtotal)",
                ["Plugins.Shipping.FixedByWeightByTotal.Fields.PercentageRateOfSubtotal.Hint"] = "Charge percentage (of subtotal).",
                ["Plugins.Shipping.FixedByWeightByTotal.Fields.Rate"] = "Rate",
                ["Plugins.Shipping.FixedByWeightByTotal.Fields.RatePerWeightUnit"] = "Rate per weight unit",
                ["Plugins.Shipping.FixedByWeightByTotal.Fields.RatePerWeightUnit.Hint"] = "Rate per weight unit.",
                ["Plugins.Shipping.FixedByWeightByTotal.Fields.ShippingMethod"] = "Shipping method",
                ["Plugins.Shipping.FixedByWeightByTotal.Fields.ShippingMethod.Hint"] = "Choose shipping method.",
                ["Plugins.Shipping.FixedByWeightByTotal.Fields.StateProvince"] = "State / province",
                ["Plugins.Shipping.FixedByWeightByTotal.Fields.StateProvince.Hint"] = "If an asterisk is selected, then this shipping rate will apply to all customers from the given country, regardless of the state.",
                ["Plugins.Shipping.FixedByWeightByTotal.Fields.Store"] = "Store",
                ["Plugins.Shipping.FixedByWeightByTotal.Fields.Store.Hint"] = "If an asterisk is selected, then this shipping rate will apply to all stores.",
                ["Plugins.Shipping.FixedByWeightByTotal.Fields.TransitDays"] = "Transit days",
                ["Plugins.Shipping.FixedByWeightByTotal.Fields.TransitDays.Hint"] = "The number of days of delivery of the goods.",
                ["Plugins.Shipping.FixedByWeightByTotal.Fields.Warehouse"] = "Warehouse",
                ["Plugins.Shipping.FixedByWeightByTotal.Fields.Warehouse.Hint"] = "If an asterisk is selected, then this shipping rate will apply to all warehouses.",
                ["Plugins.Shipping.FixedByWeightByTotal.Fields.WeightFrom"] = "Order weight from",
                ["Plugins.Shipping.FixedByWeightByTotal.Fields.WeightFrom.Hint"] = "Order weight from.",
                ["Plugins.Shipping.FixedByWeightByTotal.Fields.WeightTo"] = "Order weight to",
                ["Plugins.Shipping.FixedByWeightByTotal.Fields.WeightTo.Hint"] = "Order weight to.",
                ["Plugins.Shipping.FixedByWeightByTotal.Fields.Zip"] = "Zip",
                ["Plugins.Shipping.FixedByWeightByTotal.Fields.Zip.Hint"] = "Zip / postal code. If zip is empty, then this shipping rate will apply to all customers from the given country or state, regardless of the zip code.",
                ["Plugins.Shipping.FixedByWeightByTotal.Fixed"] = "Fixed Rate",
                ["Plugins.Shipping.FixedByWeightByTotal.Formula"] = "Formula to calculate rates",
                ["Plugins.Shipping.FixedByWeightByTotal.Formula.Value"] = "[additional fixed cost] + ([order total weight] - [lower weight limit]) * [rate per weight unit] + [order subtotal] * [charge percentage]",
                ["Plugins.Shipping.FixedByWeightByTotal.ShippingByWeight"] = "By Weight"
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
            await _settingService.DeleteSettingAsync<FixedByWeightByTotalSettings>();

            //fixed rates
            var fixedRates = await (await _shippingService.GetAllShippingMethodsAsync())
                .SelectAwait(async shippingMethod => await _settingService.GetSettingAsync(
                    string.Format(FixedByWeightByTotalDefaults.FixedRateSettingsKey, shippingMethod.Id)))
                .Where(setting => setting != null).ToListAsync();
            await _settingService.DeleteSettingsAsync(fixedRates);

            //locales
            await _localizationService.DeleteLocaleResourcesAsync("Plugins.Shipping.FixedByWeightByTotal");

            await base.UninstallAsync();
        }

        #endregion
    }
}