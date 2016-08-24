using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Routing;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Plugins;
using Nop.Plugin.Tax.CountryStateZip.Data;
using Nop.Plugin.Tax.CountryStateZip.Infrastructure.Cache;
using Nop.Plugin.Tax.CountryStateZip.Services;
using Nop.Services.Localization;
using Nop.Services.Tax;

namespace Nop.Plugin.Tax.CountryStateZip
{
    /// <summary>
    /// Fixed rate tax provider
    /// </summary>
    public class CountryStateZipTaxProvider : BasePlugin, ITaxProvider
    {
        private readonly ITaxRateService _taxRateService;
        private readonly IStoreContext _storeContext;
        private readonly TaxRateObjectContext _objectContext;
        private readonly ICacheManager _cacheManager;

        public CountryStateZipTaxProvider(ITaxRateService taxRateService,
            IStoreContext storeContext,
            TaxRateObjectContext objectContext,
            ICacheManager cacheManager)
        {
            this._taxRateService = taxRateService;
            this._storeContext = storeContext;
            this._objectContext = objectContext;
            this._cacheManager = cacheManager;
        }

        /// <summary>
        /// Gets tax rate
        /// </summary>
        /// <param name="calculateTaxRequest">Tax calculation request</param>
        /// <returns>Tax</returns>
        public CalculateTaxResult GetTaxRate(CalculateTaxRequest calculateTaxRequest)
        {
            var result = new CalculateTaxResult();

            if (calculateTaxRequest.Address == null)
            {
                result.Errors.Add("Address is not set");
                return result;
            }

            //first, load all tax rate records (cached) - loaded only once
            string cacheKey = ModelCacheEventConsumer.ALL_TAX_RATES_MODEL_KEY;
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
                    Percentage = x.Percentage,
                }
                )
                .ToList()
                );

            int storeId = _storeContext.CurrentStore.Id;
            int taxCategoryId = calculateTaxRequest.TaxCategoryId;
            int countryId = calculateTaxRequest.Address.Country != null ? calculateTaxRequest.Address.Country.Id : 0;
            int stateProvinceId = calculateTaxRequest.Address.StateProvince != null ? calculateTaxRequest.Address.StateProvince.Id : 0;
            string zip = calculateTaxRequest.Address.ZipPostalCode;


            if (zip == null)
                zip = string.Empty;
            zip = zip.Trim();

            var existingRates = new List<TaxRateForCaching>();
            foreach (var taxRate in allTaxRates)
            {
                if (taxRate.CountryId == countryId && taxRate.TaxCategoryId == taxCategoryId)
                    existingRates.Add(taxRate);
            }

            //filter by store
            var matchedByStore = new List<TaxRateForCaching>();
            //first, find by a store ID
            foreach (var taxRate in existingRates)
                if (storeId == taxRate.StoreId)
                    matchedByStore.Add(taxRate);
            //not found? use the default ones (ID == 0)
            if (!matchedByStore.Any())
                foreach (var taxRate in existingRates)
                    if (taxRate.StoreId == 0)
                        matchedByStore.Add(taxRate);


            //filter by state/province
            var matchedByStateProvince = new List<TaxRateForCaching>();
            //first, find by a state ID
            foreach (var taxRate in matchedByStore)
                if (stateProvinceId == taxRate.StateProvinceId)
                    matchedByStateProvince.Add(taxRate);
            //not found? use the default ones (ID == 0)
            if (!matchedByStateProvince.Any())
                foreach (var taxRate in matchedByStore)
                    if (taxRate.StateProvinceId == 0)
                        matchedByStateProvince.Add(taxRate);


            //filter by zip
            var matchedByZip = new List<TaxRateForCaching>();
            foreach (var taxRate in matchedByStateProvince)
                if ((String.IsNullOrEmpty(zip) && String.IsNullOrEmpty(taxRate.Zip)) ||
                    (zip.Equals(taxRate.Zip, StringComparison.InvariantCultureIgnoreCase)))
                    matchedByZip.Add(taxRate);
            if (!matchedByZip.Any())
                foreach (var taxRate in matchedByStateProvince)
                    if (String.IsNullOrWhiteSpace(taxRate.Zip))
                        matchedByZip.Add(taxRate);

            if (matchedByZip.Any())
                result.TaxRate = matchedByZip[0].Percentage;

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
            controllerName = "TaxCountryStateZip";
            routeValues = new RouteValueDictionary { { "Namespaces", "Nop.Plugin.Tax.CountryStateZip.Controllers" }, { "area", null } };
        }

        /// <summary>
        /// Install plugin
        /// </summary>
        public override void Install()
        {
            //database objects
            _objectContext.Install();

            //locales
            this.AddOrUpdatePluginLocaleResource("Plugins.Tax.CountryStateZip.Fields.Store", "Store");
            this.AddOrUpdatePluginLocaleResource("Plugins.Tax.CountryStateZip.Fields.Store.Hint", "If an asterisk is selected, then this shipping rate will apply to all stores.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Tax.CountryStateZip.Fields.Country", "Country");
            this.AddOrUpdatePluginLocaleResource("Plugins.Tax.CountryStateZip.Fields.Country.Hint", "The country.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Tax.CountryStateZip.Fields.StateProvince", "State / province");
            this.AddOrUpdatePluginLocaleResource("Plugins.Tax.CountryStateZip.Fields.StateProvince.Hint", "If an asterisk is selected, then this tax rate will apply to all customers from the given country, regardless of the state.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Tax.CountryStateZip.Fields.Zip", "Zip");
            this.AddOrUpdatePluginLocaleResource("Plugins.Tax.CountryStateZip.Fields.Zip.Hint", "Zip / postal code. If zip is empty, then this tax rate will apply to all customers from the given country or state, regardless of the zip code.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Tax.CountryStateZip.Fields.TaxCategory", "Tax category");
            this.AddOrUpdatePluginLocaleResource("Plugins.Tax.CountryStateZip.Fields.TaxCategory.Hint", "The tax category.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Tax.CountryStateZip.Fields.Percentage", "Percentage");
            this.AddOrUpdatePluginLocaleResource("Plugins.Tax.CountryStateZip.Fields.Percentage.Hint", "The tax rate.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Tax.CountryStateZip.AddRecord", "Add tax rate");
            this.AddOrUpdatePluginLocaleResource("Plugins.Tax.CountryStateZip.AddRecord.Hint", "Adding a new tax rate");

            base.Install();
        }

        /// <summary>
        /// Uninstall plugin
        /// </summary>
        public override void Uninstall()
        {
            //database objects
            _objectContext.Uninstall();

            //locales
            this.DeletePluginLocaleResource("Plugins.Tax.CountryStateZip.Fields.Store");
            this.DeletePluginLocaleResource("Plugins.Tax.CountryStateZip.Fields.Store.Hint");
            this.DeletePluginLocaleResource("Plugins.Tax.CountryStateZip.Fields.Country");
            this.DeletePluginLocaleResource("Plugins.Tax.CountryStateZip.Fields.Country.Hint");
            this.DeletePluginLocaleResource("Plugins.Tax.CountryStateZip.Fields.StateProvince");
            this.DeletePluginLocaleResource("Plugins.Tax.CountryStateZip.Fields.StateProvince.Hint");
            this.DeletePluginLocaleResource("Plugins.Tax.CountryStateZip.Fields.Zip");
            this.DeletePluginLocaleResource("Plugins.Tax.CountryStateZip.Fields.Zip.Hint");
            this.DeletePluginLocaleResource("Plugins.Tax.CountryStateZip.Fields.TaxCategory");
            this.DeletePluginLocaleResource("Plugins.Tax.CountryStateZip.Fields.TaxCategory.Hint");
            this.DeletePluginLocaleResource("Plugins.Tax.CountryStateZip.Fields.Percentage");
            this.DeletePluginLocaleResource("Plugins.Tax.CountryStateZip.Fields.Percentage.Hint");
            this.DeletePluginLocaleResource("Plugins.Tax.CountryStateZip.AddRecord");
            this.DeletePluginLocaleResource("Plugins.Tax.CountryStateZip.AddRecord.Hint");

            base.Uninstall();
        }
    }
}
