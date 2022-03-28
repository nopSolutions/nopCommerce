using System.Threading.Tasks;
using Nop.Services.Directory;
using Taxjar;
using Address = Nop.Core.Domain.Common.Address;
using Nop.Core.Caching;

namespace Nop.Plugin.Tax.AbcTax.Services
{
    public class TaxjarRateService : ITaxjarRateService
    {
        private readonly AbcTaxSettings _abcTaxSettings;
        private readonly ICountryService _countryService;
        private readonly IStaticCacheManager _staticCacheManager;

        public static CacheKey AbcTaxTaxjarRateKey =>
            new CacheKey(
                "Nop.plugin.tax.abctax.taxjarrate.{0}-{1}-{2}-{3}",
                "Nop.plugin.tax.abctax.taxjarrate."
            );

        public TaxjarRateService(
            AbcTaxSettings abcTaxSettings,
            ICountryService countryService,
            IStaticCacheManager staticCacheManager
        )
        {
            _abcTaxSettings = abcTaxSettings;
            _countryService = countryService;
            _staticCacheManager = staticCacheManager;
        }

        public async Task<decimal> GetTaxJarRateAsync(Address address)
        {
            var zip = address.ZipPostalCode?.Trim() ?? string.Empty;
            var street = address.Address1;
            var city = address.City;
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
                var taxjarApi = new TaxjarApi(_abcTaxSettings.TaxJarAPIToken);
                var rates = taxjarApi.RatesForLocation(zip, new {
                    street = street,
                    city = city,
                    country = country
                });

                // if US or Canada, CountryName will be populated
                return !string.IsNullOrWhiteSpace(rates.Country) ?
                    rates.CombinedRate * 100 :
                    rates.StandardRate * 100;
            });
        }
    }
}