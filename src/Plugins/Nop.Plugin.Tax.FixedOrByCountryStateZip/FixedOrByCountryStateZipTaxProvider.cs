using System;
using System.Linq;
using System.Web.Routing;
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
        private readonly ICountryStateZipService _taxRateService;
        private readonly IStoreContext _storeContext;
        private readonly CountryStateZipObjectContext _objectContext;
        private readonly ICacheManager _cacheManager;
        private readonly ISettingService _settingService;
        private readonly FixedOrByCountryStateZipTaxSettings _countryStateZipSettings;

        public FixedOrByCountryStateZipTaxProvider(ICountryStateZipService taxRateService,
            IStoreContext storeContext,
            CountryStateZipObjectContext objectContext,
            ICacheManager cacheManager,
            ISettingService settingService,
            FixedOrByCountryStateZipTaxSettings countryStateZipSettings)
        {
            this._taxRateService = taxRateService;
            this._storeContext = storeContext;
            this._objectContext = objectContext;
            this._cacheManager = cacheManager;
            this._settingService = settingService;
            this._countryStateZipSettings = countryStateZipSettings;
        }

        /// <summary>
        /// Gets tax rate
        /// </summary>
        /// <param name="calculateTaxRequest">Tax calculation request</param>
        /// <returns>Tax</returns>
        public CalculateTaxResult GetTaxRate(CalculateTaxRequest calculateTaxRequest)
        {
            var result = new CalculateTaxResult();

            //choose the tax rate calculation method
            if (!_countryStateZipSettings.CountryStateZipEnabled)
            {
                //the tax rate calculation by fixed rate
                result = new CalculateTaxResult
                {
                    TaxRate = _settingService.GetSettingByKey<decimal>(string.Format("Tax.TaxProvider.FixedOrByCountryStateZip.TaxCategoryId{0}", calculateTaxRequest.TaxCategoryId))
                };
            }
            else
            {
                //the tax rate calculation by country & state & zip 

                if (calculateTaxRequest.Address == null)
                {
                    result.Errors.Add("Address is not set");
                    return result;
                }

                //first, load all tax rate records (cached) - loaded only once
                const string cacheKey = ModelCacheEventConsumer.ALL_TAX_RATES_MODEL_KEY;
                var allTaxRates = _cacheManager.Get(cacheKey, () =>
                    _taxRateService
                        .GetAllTaxRates()
                        .Select(x => new TaxRateForCaching
                        {
                            Id = x.Id,
                            StoreId = x.StoreId,
                            TaxCategoryId = x.TaxCategoryId,
                            CountryId = x.CountryId,
                            StateProvinceId = x.StateProvinceId,
                            Zip = x.Zip,
                            Percentage = x.Percentage
                        }
                        )
                        .ToList()
                    );

                var storeId = _storeContext.CurrentStore.Id;
                var taxCategoryId = calculateTaxRequest.TaxCategoryId;
                var countryId = calculateTaxRequest.Address.Country != null ? calculateTaxRequest.Address.Country.Id : 0;
                var stateProvinceId = calculateTaxRequest.Address.StateProvince != null
                    ? calculateTaxRequest.Address.StateProvince.Id
                    : 0;
                var zip = calculateTaxRequest.Address.ZipPostalCode;
                
                if (zip == null)
                    zip = string.Empty;
                zip = zip.Trim();

                var existingRates = allTaxRates.Where(taxRate => taxRate.CountryId == countryId && taxRate.TaxCategoryId == taxCategoryId).ToList();

                //filter by store
                //first, find by a store ID
                var matchedByStore = existingRates.Where(taxRate => storeId == taxRate.StoreId).ToList();
                
                //not found? use the default ones (ID == 0)
                if (!matchedByStore.Any())
                    matchedByStore.AddRange(existingRates.Where(taxRate => taxRate.StoreId == 0));

                //filter by state/province
                //first, find by a state ID
                var matchedByStateProvince = matchedByStore.Where(taxRate => stateProvinceId == taxRate.StateProvinceId).ToList();
               
                //not found? use the default ones (ID == 0)
                if (!matchedByStateProvince.Any())
                    matchedByStateProvince.AddRange(matchedByStore.Where(taxRate => taxRate.StateProvinceId == 0));

                //filter by zip
                var matchedByZip = matchedByStateProvince.Where(taxRate => (string.IsNullOrEmpty(zip) && string.IsNullOrEmpty(taxRate.Zip)) || zip.Equals(taxRate.Zip, StringComparison.InvariantCultureIgnoreCase)).ToList();
                if (!matchedByZip.Any())
                    matchedByZip.AddRange(matchedByStateProvince.Where(taxRate => string.IsNullOrWhiteSpace(taxRate.Zip)));

                if (matchedByZip.Any())
                    result.TaxRate = matchedByZip[0].Percentage;
            }
            return result;
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
            controllerName = "FixedOrByCountryStateZip";
            routeValues = new RouteValueDictionary { { "Namespaces", "Nop.Plugin.Tax.FixedOrByCountryStateZip.Controllers" }, { "area", null } };
        }

        /// <summary>
        /// Install plugin
        /// </summary>
        public override void Install()
        {
            //database objects
            _objectContext.Install();

            //settings
            var settings = new FixedOrByCountryStateZipTaxSettings
            {
                CountryStateZipEnabled = false
            };
            _settingService.SaveSetting(settings);

            //locales
            this.AddOrUpdatePluginLocaleResource("Plugins.Tax.FixedOrByCountryStateZip.Fixed", "Fixed rate");
            this.AddOrUpdatePluginLocaleResource("Plugins.Tax.FixedOrByCountryStateZip.TaxByCountryStateZip", "By Country");
            this.AddOrUpdatePluginLocaleResource("Plugins.Tax.FixedOrByCountryStateZip.Fields.TaxCategoryName", "Tax category");
            this.AddOrUpdatePluginLocaleResource("Plugins.Tax.FixedOrByCountryStateZip.Fields.Rate", "Rate");
            this.AddOrUpdatePluginLocaleResource("Plugins.Tax.FixedOrByCountryStateZip.Fields.Store", "Store");
            this.AddOrUpdatePluginLocaleResource("Plugins.Tax.FixedOrByCountryStateZip.Fields.Store.Hint", "If an asterisk is selected, then this shipping rate will apply to all stores.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Tax.FixedOrByCountryStateZip.Fields.Country", "Country");
            this.AddOrUpdatePluginLocaleResource("Plugins.Tax.FixedOrByCountryStateZip.Fields.Country.Hint", "The country.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Tax.FixedOrByCountryStateZip.Fields.StateProvince", "State / province");
            this.AddOrUpdatePluginLocaleResource("Plugins.Tax.FixedOrByCountryStateZip.Fields.StateProvince.Hint", "If an asterisk is selected, then this tax rate will apply to all customers from the given country, regardless of the state.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Tax.FixedOrByCountryStateZip.Fields.Zip", "Zip");
            this.AddOrUpdatePluginLocaleResource("Plugins.Tax.FixedOrByCountryStateZip.Fields.Zip.Hint", "Zip / postal code. If zip is empty, then this tax rate will apply to all customers from the given country or state, regardless of the zip code.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Tax.FixedOrByCountryStateZip.Fields.TaxCategory", "Tax category");
            this.AddOrUpdatePluginLocaleResource("Plugins.Tax.FixedOrByCountryStateZip.Fields.TaxCategory.Hint", "The tax category.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Tax.FixedOrByCountryStateZip.Fields.Percentage", "Percentage");
            this.AddOrUpdatePluginLocaleResource("Plugins.Tax.FixedOrByCountryStateZip.Fields.Percentage.Hint", "The tax rate.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Tax.FixedOrByCountryStateZip.AddRecord", "Add tax rate");
            this.AddOrUpdatePluginLocaleResource("Plugins.Tax.FixedOrByCountryStateZip.AddRecordTitle", "New tax rate");

            base.Install();
        }

        /// <summary>
        /// Uninstall plugin
        /// </summary>
        public override void Uninstall()
        {
            //database objects
            _objectContext.Uninstall();

            //settings
            _settingService.DeleteSetting<FixedOrByCountryStateZipTaxSettings>();

            //locales
            this.DeletePluginLocaleResource("Plugins.Tax.FixedOrByCountryStateZip.Fixed");
            this.DeletePluginLocaleResource("Plugins.Tax.FixedOrByCountryStateZip.TaxByCountryStateZip");
            this.DeletePluginLocaleResource("Plugins.Tax.FixedOrByCountryStateZip.Fields.TaxCategoryName");
            this.DeletePluginLocaleResource("Plugins.Tax.FixedOrByCountryStateZip.Fields.Rate");
            this.DeletePluginLocaleResource("Plugins.Tax.FixedOrByCountryStateZip.Fields.Store");
            this.DeletePluginLocaleResource("Plugins.Tax.FixedOrByCountryStateZip.Fields.Store.Hint");
            this.DeletePluginLocaleResource("Plugins.Tax.FixedOrByCountryStateZip.Fields.Country");
            this.DeletePluginLocaleResource("Plugins.Tax.FixedOrByCountryStateZip.Fields.Country.Hint");
            this.DeletePluginLocaleResource("Plugins.Tax.FixedOrByCountryStateZip.Fields.StateProvince");
            this.DeletePluginLocaleResource("Plugins.Tax.FixedOrByCountryStateZip.Fields.StateProvince.Hint");
            this.DeletePluginLocaleResource("Plugins.Tax.FixedOrByCountryStateZip.Fields.Zip");
            this.DeletePluginLocaleResource("Plugins.Tax.FixedOrByCountryStateZip.Fields.Zip.Hint");
            this.DeletePluginLocaleResource("Plugins.Tax.FixedOrByCountryStateZip.Fields.TaxCategory");
            this.DeletePluginLocaleResource("Plugins.Tax.FixedOrByCountryStateZip.Fields.TaxCategory.Hint");
            this.DeletePluginLocaleResource("Plugins.Tax.FixedOrByCountryStateZip.Fields.Percentage");
            this.DeletePluginLocaleResource("Plugins.Tax.FixedOrByCountryStateZip.Fields.Percentage.Hint");
            this.DeletePluginLocaleResource("Plugins.Tax.FixedOrByCountryStateZip.AddRecord");
            this.DeletePluginLocaleResource("Plugins.Tax.FixedOrByCountryStateZip.AddRecordTitle");

            base.Uninstall();
        }
    }
}