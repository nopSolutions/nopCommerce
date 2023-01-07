using System.Threading.Tasks;
using Nop.Services.Directory;
using Taxjar;
using Address = Nop.Core.Domain.Common.Address;
using Nop.Core.Caching;
using Nop.Services.Tax;

namespace Nop.Plugin.Tax.AbcTax.Services
{
    public class TaxjarRateService : ITaxjarRateService
    {
        private readonly AbcTaxSettings _abcTaxSettings;
        private readonly ICountryService _countryService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly IStaticCacheManager _staticCacheManager;

        public static CacheKey AbcTaxTaxjarRateKey =>
            new CacheKey(
                "Nop.plugin.tax.abctax.taxjarrate.{0}-{1}-{2}-{3}",
                "Nop.plugin.tax.abctax.taxjarrate."
            );

        public TaxjarRateService(
            AbcTaxSettings abcTaxSettings,
            ICountryService countryService,
            IStateProvinceService stateProvinceService,
            IStaticCacheManager staticCacheManager
        )
        {
            _abcTaxSettings = abcTaxSettings;
            _countryService = countryService;
            _stateProvinceService = stateProvinceService;
            _staticCacheManager = staticCacheManager;
        }

        // https://github.com/taxjar/taxjar.net#calculate-sales-tax-for-an-order-api-docs
        public async Task<decimal> GetTaxJarRateAsync(TaxRateRequest taxRateRequest)
        {
            var address = taxRateRequest.Address;
            var zip = address.ZipPostalCode?.Trim() ?? string.Empty;
            var street = address.Address1;
            var city = address.City;
            var stateId = address.StateProvinceId.HasValue ? address.StateProvinceId.Value : 0;
            var state = await _stateProvinceService.GetStateProvinceByIdAsync(stateId);
            var stateAbbrevation = state?.Abbreviation ?? string.Empty;
            var country = (await _countryService.GetCountryByIdAsync(address.CountryId.Value))?.TwoLetterIsoCode;

            var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(
                AbcTaxTaxjarRateKey,
                zip,
                street,
                city,
                country
            );
            
            return await _staticCacheManager.GetAsync(cacheKey, () =>
            {
                var taxJarApiClient = new TaxjarApi(_abcTaxSettings.TaxJarAPIToken);
                var taxEntity = new {
                    // Always hardcoded to ABC Warehouse (worth linking to Shipping Origin?)
                    from_country = "US",
                    from_zip = "48342",
                    from_state = "MI",
                    from_city = "Pontiac",
                    from_street = "1 W Silverdome Industrial Park",
                    
                    to_country = country,
                    to_zip = zip,
                    to_state = stateAbbrevation,
                    to_city = city,
                    to_street = street,

                    amount = taxRateRequest.Price,
                    // Since we're just getting the rate, we don't need this
                    shipping = 0
                };
                var tax = taxJarApiClient.TaxForOrder(taxEntity);

                return tax.Rate * 100;
            });
        }
    }
}