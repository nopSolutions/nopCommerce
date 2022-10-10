using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Tax;
using Nop.Plugin.Tax.AbcTax.Domain;
using Nop.Plugin.Tax.AbcTax.Infrastructure.Cache;
using Nop.Plugin.Tax.AbcTax.Services;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Plugins;
using Nop.Services.Tax;
using Nop.Data;
using Nop.Services.Directory;
using Taxjar;
using Nop.Services.Logging;

namespace Nop.Plugin.Tax.AbcTax
{
    public class AbcTaxProvider : BasePlugin, ITaxProvider
    {
        private readonly AbcTaxSettings _abcTaxSettings;
        private readonly IAbcTaxService _abcTaxService;
        private readonly ICountryService _countryService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILocalizationService _localizationService;
        private readonly INopDataProvider _nopDataProvider;
        private readonly IOrderTotalCalculationService _orderTotalCalculationService;
        private readonly IPaymentService _paymentService;
        private readonly ISettingService _settingService;
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly ITaxCategoryService _taxCategoryService;
        private readonly ITaxjarRateService _taxjarRateService;
        private readonly ITaxService _taxService;
        private readonly IWebHelper _webHelper;
        private readonly TaxSettings _taxSettings;
        private readonly ILogger _logger;

        public AbcTaxProvider(AbcTaxSettings abcTaxSettings,
            IAbcTaxService abcTaxService,
            ICountryService countryService,
            IGenericAttributeService genericAttributeService,
            IHttpContextAccessor httpContextAccessor,
            ILocalizationService localizationService,
            INopDataProvider nopDataProvider,
            IOrderTotalCalculationService orderTotalCalculationService,
            IPaymentService paymentService,
            ISettingService settingService,
            IStaticCacheManager staticCacheManager,
            ITaxCategoryService taxCategoryService,
            ITaxjarRateService taxjarRateService,
            ITaxService taxService,
            IWebHelper webHelper,
            TaxSettings taxSettings,
            ILogger logger)
        {
            _abcTaxSettings = abcTaxSettings;
            _abcTaxService = abcTaxService;
            _countryService = countryService;
            _genericAttributeService = genericAttributeService;
            _httpContextAccessor = httpContextAccessor;
            _localizationService = localizationService;
            _nopDataProvider = nopDataProvider;
            _orderTotalCalculationService = orderTotalCalculationService;
            _paymentService = paymentService;
            _settingService = settingService;
            _staticCacheManager = staticCacheManager;
            _taxCategoryService = taxCategoryService;
            _taxjarRateService = taxjarRateService;
            _taxService = taxService;
            _webHelper = webHelper;
            _taxSettings = taxSettings;
            _logger = logger;
        }

        public async Task<TaxRateResult> GetTaxRateAsync(TaxRateRequest taxRateRequest)
        {
            var result = new TaxRateResult();

            //the tax rate calculation by country & state & zip 
            if (taxRateRequest.Address == null)
            {
                result.Errors.Add("Address is not set");
                return result;
            }

            var foundRecord = await _abcTaxService.GetAbcTaxRateAsync(
                taxRateRequest.CurrentStoreId,
                taxRateRequest.TaxCategoryId,
                taxRateRequest.Address
            );

            if (_abcTaxSettings.IsDebugMode && foundRecord != null)
            {
                await _logger.InformationAsync($"TaxJar Enabled: {foundRecord.IsTaxJarEnabled}");
                await _logger.InformationAsync($"Percentage: {foundRecord.Percentage}");
                await _logger.InformationAsync($"TaxCategoryId: {foundRecord.TaxCategoryId}");
            }

            if (foundRecord == null)
            {
                if (_abcTaxSettings.IsDebugMode)
                {
                    await _logger.InformationAsync($"Record not found for currentStoreId: {taxRateRequest.CurrentStoreId}, taxCategoryId: {taxRateRequest.TaxCategoryId}, address: {taxRateRequest.Address}");
                }

                return result;
            }

            // get TaxJar rate if appropriate
            result.TaxRate = foundRecord.IsTaxJarEnabled ?
                await _taxjarRateService.GetTaxJarRateAsync(taxRateRequest.Address) :
                foundRecord.Percentage;

            if (_abcTaxSettings.IsDebugMode)
            {
                await _logger.InformationAsync($"Tax Rate: {result.TaxRate}");
            }
            
            return result;
        }

        public async Task<TaxTotalResult> GetTaxTotalAsync(TaxTotalRequest taxTotalRequest)
        {
            if (_httpContextAccessor.HttpContext.Items.TryGetValue("nop.TaxTotal", out var result)
                && result is (TaxTotalResult taxTotalResult, decimal paymentTax))
            {
                //short-circuit to avoid circular reference when calculating payment method additional fee during the checkout process
                if (!taxTotalRequest.UsePaymentMethodAdditionalFee)
                    return new TaxTotalResult { TaxTotal = taxTotalResult.TaxTotal - paymentTax };

                return taxTotalResult;
            }

            var taxRates = new SortedDictionary<decimal, decimal>();
            var taxTotal = decimal.Zero;

            taxTotal += await GetSubtotalTaxTotalAsync(taxRates, taxTotalRequest);
            await _logger.InformationAsync($"taxTotal (AbcTaxProvider, subtotal): {taxTotal}");
            taxTotal += await GetShippingTaxAsync(taxRates, taxTotalRequest);
            await _logger.InformationAsync($"taxTotal (AbcTaxProvider, shipping): {taxTotal}");

            //short-circuit to avoid circular reference when calculating payment method additional fee during the checkout process
            if (!taxTotalRequest.UsePaymentMethodAdditionalFee)
                return new TaxTotalResult { TaxTotal = taxTotal };

            var paymentMethodAdditionalFeeTax = await GetPaymentMethodAdditionalFeeTaxAsync(taxRates, taxTotalRequest);
            taxTotal += paymentMethodAdditionalFeeTax;

            //add at least one tax rate (0%)
            if (!taxRates.Any())
                taxRates.Add(decimal.Zero, decimal.Zero);

            if (taxTotal < decimal.Zero)
                taxTotal = decimal.Zero;

            taxTotalResult = new TaxTotalResult { TaxTotal = taxTotal, TaxRates = taxRates, };

            //store values within the scope of the request to avoid duplicate calculations
            _httpContextAccessor.HttpContext.Items.TryAdd("nop.TaxTotal", (taxTotalResult, paymentMethodAdditionalFeeTax));

            return taxTotalResult;
        }

        public override string GetConfigurationPageUrl()
        {
            return $"{_webHelper.GetStoreLocation()}Admin/AbcTax/Configure";
        }

        public override async Task InstallAsync()
        {
            //settings
            await _settingService.SaveSettingAsync(new AbcTaxSettings());

            //locales
            await UpdateLocales();

            // If possible, import data from old Tax plugin
            await _nopDataProvider.ExecuteNonQueryAsync($@"
                INSERT INTO AbcTaxRate (StoreId, TaxCategoryId, CountryId, StateProvinceId, Zip, Percentage, IsTaxJarEnabled)
                SELECT
                    StoreId,
                    TaxCategoryId,
                    CountryId,
                    StateProvinceId,
                    Zip,
                    Percentage,
                    EnableTaxState
                FROM
                    TaxRate
            ");

            await base.InstallAsync();
        }

        public override async Task UninstallAsync()
        {
            //settings
            await _settingService.DeleteSettingAsync<AbcTaxSettings>();

            //locales
            await _localizationService.DeleteLocaleResourcesAsync("Plugins.Tax.AbcTax");

            await base.UninstallAsync();
        }

        public override async Task UpdateAsync(string oldVersion, string currentVersion)
        {
            //locales
            await UpdateLocales();

            await base.UpdateAsync(oldVersion, currentVersion);
        }

        private async Task UpdateLocales()
        {
            await _localizationService.AddLocaleResourceAsync(new Dictionary<string, string>
            {
                ["Plugins.Tax.AbcTax.Tax.Categories.Manage"] = "Manage tax categories",
                ["Plugins.Tax.AbcTax.TaxCategoriesCanNotLoaded"] = "No tax categories can be loaded. You may manage tax categories by <a href='{0}'>this link</a>",
                ["Plugins.Tax.AbcTax.TaxByCountryStateZip"] = "By Country",
                ["Plugins.Tax.AbcTax.Fields.TaxCategoryName"] = "Tax category",
                ["Plugins.Tax.AbcTax.Fields.Rate"] = "Rate",
                ["Plugins.Tax.AbcTax.Fields.Store"] = "Store",
                ["Plugins.Tax.AbcTax.Fields.Store.Hint"] = "If an asterisk is selected, then this shipping rate will apply to all stores.",
                ["Plugins.Tax.AbcTax.Fields.Country"] = "Country",
                ["Plugins.Tax.AbcTax.Fields.Country.Hint"] = "The country.",
                ["Plugins.Tax.AbcTax.Fields.StateProvince"] = "State / province",
                ["Plugins.Tax.AbcTax.Fields.StateProvince.Hint"] = "If an asterisk is selected, then this tax rate will apply to all customers from the given country, regardless of the state.",
                ["Plugins.Tax.AbcTax.Fields.Zip"] = "Zip",
                ["Plugins.Tax.AbcTax.Fields.Zip.Hint"] = "Zip / postal code. If zip is empty, then this tax rate will apply to all customers from the given country or state, regardless of the zip code.",
                ["Plugins.Tax.AbcTax.Fields.TaxCategory"] = "Tax category",
                ["Plugins.Tax.AbcTax.Fields.TaxCategory.Hint"] = "The tax category.",
                ["Plugins.Tax.AbcTax.Fields.Percentage"] = "Percentage",
                ["Plugins.Tax.AbcTax.Fields.Percentage.Hint"] = "The tax rate.",
                ["Plugins.Tax.AbcTax.Fields.IsTaxJarEnabled"] = "Is TaxJar enabled",
                ["Plugins.Tax.AbcTax.Fields.IsTaxJarEnabled.Hint"] = "Whether the rate is enabled.",
                ["Plugins.Tax.AbcTax.Fields.TaxJarAPIToken"] = "TaxJar API Token",
                ["Plugins.Tax.AbcTax.Fields.TaxJarAPIToken.Hint"] = "Whether the rate is enabled.",
                ["Plugins.Tax.AbcTax.Fields.IsDebugMode"] = "Debug Mode Enabled",
                ["Plugins.Tax.AbcTax.Fields.IsDebugMode.Hint"] = "Enables debugging in logs.",
                ["Plugins.Tax.AbcTax.AddRecord"] = "Add tax rate",
                ["Plugins.Tax.AbcTax.AddRecordTitle"] = "New tax rate"
            });
        }

        private async Task<decimal> GetShippingTaxAsync(
            SortedDictionary<decimal, decimal> taxRates,
            TaxTotalRequest taxTotalRequest
        ) {
            var shippingTax = decimal.Zero;
            if (_taxSettings.ShippingIsTaxable)
            {
                var (shippingExclTax, _, _) = await _orderTotalCalculationService
                    .GetShoppingCartShippingTotalAsync(taxTotalRequest.ShoppingCart, false);
                var (shippingInclTax, taxRate, _) = await _orderTotalCalculationService
                    .GetShoppingCartShippingTotalAsync(taxTotalRequest.ShoppingCart, true);
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

            return shippingTax;
        }

        private async Task<decimal> GetPaymentMethodAdditionalFeeTaxAsync(
            SortedDictionary<decimal, decimal> taxRates,
            TaxTotalRequest taxTotalRequest
        )
        {
            var paymentMethodAdditionalFeeTax = decimal.Zero;
            if (_taxSettings.PaymentMethodAdditionalFeeIsTaxable)
            {
                var paymentMethodSystemName = taxTotalRequest.Customer != null
                    ? await _genericAttributeService
                        .GetAttributeAsync<string>(taxTotalRequest.Customer, NopCustomerDefaults.SelectedPaymentMethodAttribute, taxTotalRequest.StoreId)
                    : string.Empty;

                var paymentMethodAdditionalFee = await _paymentService
                    .GetAdditionalHandlingFeeAsync(taxTotalRequest.ShoppingCart, paymentMethodSystemName);
                var (paymentMethodAdditionalFeeExclTax, _) = await _taxService
                    .GetPaymentMethodAdditionalFeeAsync(paymentMethodAdditionalFee, false, taxTotalRequest.Customer);
                var (paymentMethodAdditionalFeeInclTax, taxRate) = await _taxService
                    .GetPaymentMethodAdditionalFeeAsync(paymentMethodAdditionalFee, true, taxTotalRequest.Customer);

                paymentMethodAdditionalFeeTax = paymentMethodAdditionalFeeInclTax - paymentMethodAdditionalFeeExclTax;
                if (paymentMethodAdditionalFeeTax < decimal.Zero)
                    paymentMethodAdditionalFeeTax = decimal.Zero;

                if (taxRate > decimal.Zero && paymentMethodAdditionalFeeTax > decimal.Zero)
                {
                    if (!taxRates.ContainsKey(taxRate))
                        taxRates.Add(taxRate, paymentMethodAdditionalFeeTax);
                    else
                        taxRates[taxRate] = taxRates[taxRate] + paymentMethodAdditionalFeeTax;
                }
            }
            return paymentMethodAdditionalFeeTax;
        }

        private async Task<decimal> GetSubtotalTaxTotalAsync(
            SortedDictionary<decimal, decimal> taxRates,
            TaxTotalRequest taxTotalRequest
        )
        {
            var (_, _, _, _, orderSubTotalTaxRates) = await _orderTotalCalculationService
                .GetShoppingCartSubTotalAsync(taxTotalRequest.ShoppingCart, false);
            await _logger.InformationAsync($"orderSubTotalTaxRates: {orderSubTotalTaxRates.FirstOrDefault()}");
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
            return subTotalTaxTotal;
        }
    }
}