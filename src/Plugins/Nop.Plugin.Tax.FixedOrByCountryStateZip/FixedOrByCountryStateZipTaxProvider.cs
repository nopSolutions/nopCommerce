using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Tax;
using Nop.Plugin.Tax.FixedOrByCountryStateZip.Domain;
using Nop.Plugin.Tax.FixedOrByCountryStateZip.Infrastructure.Cache;
using Nop.Plugin.Tax.FixedOrByCountryStateZip.Services;
using Nop.Services.Caching;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Plugins;
using Nop.Services.Tax;

namespace Nop.Plugin.Tax.FixedOrByCountryStateZip
{
    /// <summary>
    /// Fixed or by country & state & zip rate tax provider
    /// </summary>
    public class FixedOrByCountryStateZipTaxProvider : BasePlugin, ITaxProvider
    {
        #region Fields

        private readonly FixedOrByCountryStateZipTaxSettings _countryStateZipSettings;
        private readonly ICacheKeyService _cacheKeyService;
        private readonly ICountryStateZipService _taxRateService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILocalizationService _localizationService;
        private readonly IOrderTotalCalculationService _orderTotalCalculationService;
        private readonly IPaymentService _paymentService;
        private readonly ISettingService _settingService;
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly ITaxCategoryService _taxCategoryService;
        private readonly ITaxService _taxService;
        private readonly IWebHelper _webHelper;
        private readonly TaxSettings _taxSettings;

        #endregion

        #region Ctor

        public FixedOrByCountryStateZipTaxProvider(FixedOrByCountryStateZipTaxSettings countryStateZipSettings,
            ICacheKeyService cacheKeyService,
            ICountryStateZipService taxRateService,
            IGenericAttributeService genericAttributeService,
            IHttpContextAccessor httpContextAccessor,
            ILocalizationService localizationService,
            IOrderTotalCalculationService orderTotalCalculationService,
            IPaymentService paymentService,
            ISettingService settingService,
            IStaticCacheManager staticCacheManager,
            ITaxCategoryService taxCategoryService,
            ITaxService taxService,
            IWebHelper webHelper,
            TaxSettings taxSettings)
        {
            _countryStateZipSettings = countryStateZipSettings;
            _cacheKeyService = cacheKeyService;
            _taxRateService = taxRateService;
            _genericAttributeService = genericAttributeService;
            _httpContextAccessor = httpContextAccessor;
            _localizationService = localizationService;
            _orderTotalCalculationService = orderTotalCalculationService;
            _paymentService = paymentService;
            _settingService = settingService;
            _staticCacheManager = staticCacheManager;
            _taxCategoryService = taxCategoryService;
            _taxService = taxService;
            _webHelper = webHelper;
            _taxSettings = taxSettings;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets tax rate
        /// </summary>
        /// <param name="taxRateRequest">Tax rate request</param>
        /// <returns>Tax</returns>
        public TaxRateResult GetTaxRate(TaxRateRequest taxRateRequest)
        {
            var result = new TaxRateResult();

            //the tax rate calculation by fixed rate
            if (!_countryStateZipSettings.CountryStateZipEnabled)
            {
                result.TaxRate = _settingService.GetSettingByKey<decimal>(string.Format(FixedOrByCountryStateZipDefaults.FixedRateSettingsKey, taxRateRequest.TaxCategoryId));
                return result;
            }

            //the tax rate calculation by country & state & zip 
            if (taxRateRequest.Address == null)
            {
                result.Errors.Add("Address is not set");
                return result;
            }

            //first, load all tax rate records (cached) - loaded only once
            var cacheKey = _cacheKeyService.PrepareKeyForDefaultCache(ModelCacheEventConsumer.ALL_TAX_RATES_MODEL_KEY);
            var allTaxRates = _staticCacheManager.Get(cacheKey, () => _taxRateService.GetAllTaxRates().Select(taxRate => new TaxRate
            {
                Id = taxRate.Id,
                StoreId = taxRate.StoreId,
                TaxCategoryId = taxRate.TaxCategoryId,
                CountryId = taxRate.CountryId,
                StateProvinceId = taxRate.StateProvinceId,
                Zip = taxRate.Zip,
                Percentage = taxRate.Percentage
            }).ToList());

            var storeId = taxRateRequest.CurrentStoreId;
            var taxCategoryId = taxRateRequest.TaxCategoryId;
            var countryId = taxRateRequest.Address.CountryId;
            var stateProvinceId = taxRateRequest.Address.StateProvinceId;
            var zip = taxRateRequest.Address.ZipPostalCode?.Trim() ?? string.Empty;

            var existingRates = allTaxRates.Where(taxRate => taxRate.CountryId == countryId && taxRate.TaxCategoryId == taxCategoryId);

            //filter by store
            var matchedByStore = existingRates.Where(taxRate => storeId == taxRate.StoreId || taxRate.StoreId == 0);

            //filter by state/province
            var matchedByStateProvince = matchedByStore.Where(taxRate => stateProvinceId == taxRate.StateProvinceId || taxRate.StateProvinceId == 0);

            //filter by zip
            var matchedByZip = matchedByStateProvince.Where(taxRate => string.IsNullOrWhiteSpace(taxRate.Zip) || taxRate.Zip.Equals(zip, StringComparison.InvariantCultureIgnoreCase));

            //sort from particular to general, more particular cases will be the first
            var foundRecords = matchedByZip.OrderBy(r => r.StoreId == 0).ThenBy(r => r.StateProvinceId == 0).ThenBy(r => string.IsNullOrEmpty(r.Zip));

            var foundRecord = foundRecords.FirstOrDefault();

            if (foundRecord != null)
                result.TaxRate = foundRecord.Percentage;

            return result;
        }

        /// <summary>
        /// Gets tax total
        /// </summary>
        /// <param name="taxTotalRequest">Tax total request</param>
        /// <returns>Tax total</returns>
        public TaxTotalResult GetTaxTotal(TaxTotalRequest taxTotalRequest)
        {
            if (!(_httpContextAccessor.HttpContext.Items.TryGetValue("nop.TaxTotal", out var result) && result is TaxTotalResult taxTotalResult))
            {
                var taxRates = new SortedDictionary<decimal, decimal>();

                //order sub total (items + checkout attributes)
                _orderTotalCalculationService
                    .GetShoppingCartSubTotal(taxTotalRequest.ShoppingCart, false, out _, out _, out _, out _, out var orderSubTotalTaxRates);
                var subTotalTaxTotal = decimal.Zero;
                foreach (var kvp in orderSubTotalTaxRates)
                {
                    var taxRate = kvp.Key;
                    var taxValue = kvp.Value;
                    subTotalTaxTotal += taxValue;

                    if (taxRate > decimal.Zero && taxValue > decimal.Zero)
                    {
                        if (!taxRates.ContainsKey(taxRate))
                            taxRates.Add(taxRate, taxValue);
                        else
                            taxRates[taxRate] = taxRates[taxRate] + taxValue;
                    }
                }

                //shipping
                var shippingTax = decimal.Zero;
                if (_taxSettings.ShippingIsTaxable)
                {
                    var shippingExclTax = _orderTotalCalculationService
                        .GetShoppingCartShippingTotal(taxTotalRequest.ShoppingCart, false, out _);
                    var shippingInclTax = _orderTotalCalculationService
                        .GetShoppingCartShippingTotal(taxTotalRequest.ShoppingCart, true, out var taxRate);
                    if (shippingExclTax.HasValue && shippingInclTax.HasValue)
                    {
                        shippingTax = shippingInclTax.Value - shippingExclTax.Value;
                        if (shippingTax < decimal.Zero)
                            shippingTax = decimal.Zero;

                        if (taxRate > decimal.Zero && shippingTax > decimal.Zero)
                        {
                            if (!taxRates.ContainsKey(taxRate))
                                taxRates.Add(taxRate, shippingTax);
                            else
                                taxRates[taxRate] = taxRates[taxRate] + shippingTax;
                        }
                    }
                }

                //add at least one tax rate (0%)
                if (!taxRates.Any())
                    taxRates.Add(decimal.Zero, decimal.Zero);

                var taxTotal = subTotalTaxTotal + shippingTax;

                if (taxTotal < decimal.Zero)
                    taxTotal = decimal.Zero;

                taxTotalResult = new TaxTotalResult { TaxTotal = taxTotal, TaxRates = taxRates };
                _httpContextAccessor.HttpContext.Items.TryAdd("nop.TaxTotal", taxTotalResult);
            }

            //payment method additional fee
            if (taxTotalRequest.UsePaymentMethodAdditionalFee && _taxSettings.PaymentMethodAdditionalFeeIsTaxable)
            {
                var paymentMethodSystemName = taxTotalRequest.Customer != null
                    ? _genericAttributeService.GetAttribute<string>(taxTotalRequest.Customer,
                        NopCustomerDefaults.SelectedPaymentMethodAttribute, taxTotalRequest.StoreId)
                    : string.Empty;
                var paymentMethodAdditionalFee = _paymentService.GetAdditionalHandlingFee(taxTotalRequest.ShoppingCart, paymentMethodSystemName);
                var paymentMethodAdditionalFeeExclTax = _taxService
                    .GetPaymentMethodAdditionalFee(paymentMethodAdditionalFee, false, taxTotalRequest.Customer, out _);
                var paymentMethodAdditionalFeeInclTax = _taxService
                    .GetPaymentMethodAdditionalFee(paymentMethodAdditionalFee, true, taxTotalRequest.Customer, out var taxRate);
                var paymentMethodAdditionalFeeTax = paymentMethodAdditionalFeeInclTax - paymentMethodAdditionalFeeExclTax;

                if (paymentMethodAdditionalFeeTax < decimal.Zero)
                    paymentMethodAdditionalFeeTax = decimal.Zero;

                taxTotalResult.TaxTotal += paymentMethodAdditionalFeeTax;

                if (taxRate > decimal.Zero && paymentMethodAdditionalFeeTax > decimal.Zero)
                {
                    if (!taxTotalResult.TaxRates.ContainsKey(taxRate))
                        taxTotalResult.TaxRates.Add(taxRate, paymentMethodAdditionalFeeTax);
                    else
                        taxTotalResult.TaxRates[taxRate] = taxTotalResult.TaxRates[taxRate] + paymentMethodAdditionalFeeTax;
                }
            }

            return taxTotalResult;
        }

        /// <summary>
        /// Gets a configuration page URL
        /// </summary>
        public override string GetConfigurationPageUrl()
        {
            return $"{_webHelper.GetStoreLocation()}Admin/FixedOrByCountryStateZip/Configure";
        }

        /// <summary>
        /// Install plugin
        /// </summary>
        public override void Install()
        {
            //settings
            _settingService.SaveSetting(new FixedOrByCountryStateZipTaxSettings());

            //locales
            _localizationService.AddPluginLocaleResource(new Dictionary<string, string>
            {
                ["Plugins.Tax.FixedOrByCountryStateZip.Fixed"] = "Fixed rate",
                ["Plugins.Tax.FixedOrByCountryStateZip.TaxByCountryStateZip"] = "By Country",
                ["Plugins.Tax.FixedOrByCountryStateZip.Fields.TaxCategoryName"] = "Tax category",
                ["Plugins.Tax.FixedOrByCountryStateZip.Fields.Rate"] = "Rate",
                ["Plugins.Tax.FixedOrByCountryStateZip.Fields.Store"] = "Store",
                ["Plugins.Tax.FixedOrByCountryStateZip.Fields.Store.Hint"] = "If an asterisk is selected, then this shipping rate will apply to all stores.",
                ["Plugins.Tax.FixedOrByCountryStateZip.Fields.Country"] = "Country",
                ["Plugins.Tax.FixedOrByCountryStateZip.Fields.Country.Hint"] = "The country.",
                ["Plugins.Tax.FixedOrByCountryStateZip.Fields.StateProvince"] = "State / province",
                ["Plugins.Tax.FixedOrByCountryStateZip.Fields.StateProvince.Hint"] = "If an asterisk is selected, then this tax rate will apply to all customers from the given country, regardless of the state.",
                ["Plugins.Tax.FixedOrByCountryStateZip.Fields.Zip"] = "Zip",
                ["Plugins.Tax.FixedOrByCountryStateZip.Fields.Zip.Hint"] = "Zip / postal code. If zip is empty, then this tax rate will apply to all customers from the given country or state, regardless of the zip code.",
                ["Plugins.Tax.FixedOrByCountryStateZip.Fields.TaxCategory"] = "Tax category",
                ["Plugins.Tax.FixedOrByCountryStateZip.Fields.TaxCategory.Hint"] = "The tax category.",
                ["Plugins.Tax.FixedOrByCountryStateZip.Fields.Percentage"] = "Percentage",
                ["Plugins.Tax.FixedOrByCountryStateZip.Fields.Percentage.Hint"] = "The tax rate.",
                ["Plugins.Tax.FixedOrByCountryStateZip.AddRecord"] = "Add tax rate",
                ["Plugins.Tax.FixedOrByCountryStateZip.AddRecordTitle"] = "New tax rate"
            });

            base.Install();
        }

        /// <summary>
        /// Uninstall plugin
        /// </summary>
        public override void Uninstall()
        {
            //settings
            _settingService.DeleteSetting<FixedOrByCountryStateZipTaxSettings>();

            //fixed rates
            var fixedRates = _taxCategoryService.GetAllTaxCategories()
                .Select(taxCategory => _settingService.GetSetting(string.Format(FixedOrByCountryStateZipDefaults.FixedRateSettingsKey, taxCategory.Id)))
                .Where(setting => setting != null).ToList();
            _settingService.DeleteSettings(fixedRates);

            //locales
            _localizationService.DeletePluginLocaleResources("Plugins.Tax.FixedOrByCountryStateZip");

            base.Uninstall();
        }

        #endregion
    }
}