using System;
using System.Linq;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Plugins;
using Nop.Plugin.Tax.FixedOrByCountryStateZip.Data;
using Nop.Plugin.Tax.FixedOrByCountryStateZip.Infrastructure.Cache;
using Nop.Plugin.Tax.FixedOrByCountryStateZip.Services;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Tax;

namespace Nop.Plugin.Tax.FixedOrByCountryStateZip
{
    /// <summary>
    /// Fixed or by country & state & zip rate tax provider
    /// </summary>
    public class FixedOrByCountryStateZipTaxProvider : BasePlugin, ITaxProvider
    {
        #region Fields

        private readonly CountryStateZipObjectContext _objectContext;
        private readonly FixedOrByCountryStateZipTaxSettings _countryStateZipSettings;
        private readonly ICountryStateZipService _taxRateService;
        private readonly ILocalizationService _localizationService;
        private readonly ISettingService _settingService;
        private readonly IStaticCacheManager _cacheManager;
        private readonly ITaxCategoryService _taxCategoryService;
        private readonly IWebHelper _webHelper;

        #endregion

        #region Ctor

        public FixedOrByCountryStateZipTaxProvider(CountryStateZipObjectContext objectContext,
            FixedOrByCountryStateZipTaxSettings countryStateZipSettings,
            ICountryStateZipService taxRateService,
            ILocalizationService localizationService,
            ISettingService settingService,
            IStaticCacheManager cacheManager,
            ITaxCategoryService taxCategoryService,
            IWebHelper webHelper)
        {
            this._objectContext = objectContext;
            this._countryStateZipSettings = countryStateZipSettings;
            this._taxRateService = taxRateService;
            this._localizationService = localizationService;
            this._settingService = settingService;
            this._cacheManager = cacheManager;
            this._taxCategoryService = taxCategoryService;
            this._webHelper = webHelper;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets tax rate
        /// </summary>
        /// <param name="calculateTaxRequest">Tax calculation request</param>
        /// <returns>Tax</returns>
        public CalculateTaxResult GetTaxRate(CalculateTaxRequest calculateTaxRequest)
        {
            var result = new CalculateTaxResult();

            //the tax rate calculation by fixed rate
            if (!_countryStateZipSettings.CountryStateZipEnabled)
            {
                result.TaxRate = _settingService.GetSettingByKey<decimal>(string.Format(FixedOrByCountryStateZipDefaults.FixedRateSettingsKey, calculateTaxRequest.TaxCategoryId));
                return result;
            }

            //the tax rate calculation by country & state & zip 
            if (calculateTaxRequest.Address == null)
            {
                result.Errors.Add("Address is not set");
                return result;
            }

            //first, load all tax rate records (cached) - loaded only once
            var cacheKey = ModelCacheEventConsumer.ALL_TAX_RATES_MODEL_KEY;
            var allTaxRates = _cacheManager.Get(cacheKey, () => _taxRateService.GetAllTaxRates().Select(taxRate => new TaxRateForCaching
            {
                Id = taxRate.Id,
                StoreId = taxRate.StoreId,
                TaxCategoryId = taxRate.TaxCategoryId,
                CountryId = taxRate.CountryId,
                StateProvinceId = taxRate.StateProvinceId,
                Zip = taxRate.Zip,
                Percentage = taxRate.Percentage
            }).ToList());

            var storeId = calculateTaxRequest.CurrentStoreId;
            var taxCategoryId = calculateTaxRequest.TaxCategoryId;
            var countryId = calculateTaxRequest.Address.Country?.Id ?? 0;
            var stateProvinceId = calculateTaxRequest.Address.StateProvince?.Id ?? 0;
            var zip = calculateTaxRequest.Address.ZipPostalCode?.Trim() ?? string.Empty;

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
            //database objects
            _objectContext.Install();

            //settings
            _settingService.SaveSetting(new FixedOrByCountryStateZipTaxSettings());

            //locales
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Tax.FixedOrByCountryStateZip.Fixed", "Fixed rate");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Tax.FixedOrByCountryStateZip.TaxByCountryStateZip", "By Country");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Tax.FixedOrByCountryStateZip.Fields.TaxCategoryName", "Tax category");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Tax.FixedOrByCountryStateZip.Fields.Rate", "Rate");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Tax.FixedOrByCountryStateZip.Fields.Store", "Store");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Tax.FixedOrByCountryStateZip.Fields.Store.Hint", "If an asterisk is selected, then this shipping rate will apply to all stores.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Tax.FixedOrByCountryStateZip.Fields.Country", "Country");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Tax.FixedOrByCountryStateZip.Fields.Country.Hint", "The country.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Tax.FixedOrByCountryStateZip.Fields.StateProvince", "State / province");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Tax.FixedOrByCountryStateZip.Fields.StateProvince.Hint", "If an asterisk is selected, then this tax rate will apply to all customers from the given country, regardless of the state.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Tax.FixedOrByCountryStateZip.Fields.Zip", "Zip");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Tax.FixedOrByCountryStateZip.Fields.Zip.Hint", "Zip / postal code. If zip is empty, then this tax rate will apply to all customers from the given country or state, regardless of the zip code.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Tax.FixedOrByCountryStateZip.Fields.TaxCategory", "Tax category");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Tax.FixedOrByCountryStateZip.Fields.TaxCategory.Hint", "The tax category.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Tax.FixedOrByCountryStateZip.Fields.Percentage", "Percentage");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Tax.FixedOrByCountryStateZip.Fields.Percentage.Hint", "The tax rate.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Tax.FixedOrByCountryStateZip.AddRecord", "Add tax rate");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Tax.FixedOrByCountryStateZip.AddRecordTitle", "New tax rate");

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

            //database objects
            _objectContext.Uninstall();

            //locales
            _localizationService.DeletePluginLocaleResource("Plugins.Tax.FixedOrByCountryStateZip.Fixed");
            _localizationService.DeletePluginLocaleResource("Plugins.Tax.FixedOrByCountryStateZip.TaxByCountryStateZip");
            _localizationService.DeletePluginLocaleResource("Plugins.Tax.FixedOrByCountryStateZip.Fields.TaxCategoryName");
            _localizationService.DeletePluginLocaleResource("Plugins.Tax.FixedOrByCountryStateZip.Fields.Rate");
            _localizationService.DeletePluginLocaleResource("Plugins.Tax.FixedOrByCountryStateZip.Fields.Store");
            _localizationService.DeletePluginLocaleResource("Plugins.Tax.FixedOrByCountryStateZip.Fields.Store.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Tax.FixedOrByCountryStateZip.Fields.Country");
            _localizationService.DeletePluginLocaleResource("Plugins.Tax.FixedOrByCountryStateZip.Fields.Country.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Tax.FixedOrByCountryStateZip.Fields.StateProvince");
            _localizationService.DeletePluginLocaleResource("Plugins.Tax.FixedOrByCountryStateZip.Fields.StateProvince.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Tax.FixedOrByCountryStateZip.Fields.Zip");
            _localizationService.DeletePluginLocaleResource("Plugins.Tax.FixedOrByCountryStateZip.Fields.Zip.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Tax.FixedOrByCountryStateZip.Fields.TaxCategory");
            _localizationService.DeletePluginLocaleResource("Plugins.Tax.FixedOrByCountryStateZip.Fields.TaxCategory.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Tax.FixedOrByCountryStateZip.Fields.Percentage");
            _localizationService.DeletePluginLocaleResource("Plugins.Tax.FixedOrByCountryStateZip.Fields.Percentage.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Tax.FixedOrByCountryStateZip.AddRecord");
            _localizationService.DeletePluginLocaleResource("Plugins.Tax.FixedOrByCountryStateZip.AddRecordTitle");

            base.Uninstall();
        }

        #endregion
    }
}