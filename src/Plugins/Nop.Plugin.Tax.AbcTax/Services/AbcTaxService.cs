using System.Linq;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Data;
using Nop.Plugin.Tax.AbcTax.Domain;
using Nop.Plugin.Tax.AbcTax.Infrastructure.Cache;
using Nop.Core.Caching;
using Nop.Services.Tax;
using Nop.Core.Domain.Common;
using System;

namespace Nop.Plugin.Tax.AbcTax.Services
{
    public partial class AbcTaxService : IAbcTaxService
    {
        private readonly IRepository<AbcTaxRate> _abcTaxRateRepository;

        private readonly IStaticCacheManager _staticCacheManager;

        public AbcTaxService(
            IRepository<AbcTaxRate> abcTaxRateRepository,
            IStaticCacheManager staticCacheManager
        ) {
            _abcTaxRateRepository = abcTaxRateRepository;
            _staticCacheManager = staticCacheManager;
        }

        public virtual async Task DeleteTaxRateAsync(AbcTaxRate taxRate)
        {
            await _abcTaxRateRepository.DeleteAsync(taxRate);
        }

        public virtual async Task<IPagedList<AbcTaxRate>> GetAllTaxRatesAsync(int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var rez = await _abcTaxRateRepository.GetAllAsync(query =>
            {
                return from tr in query
                    orderby tr.StoreId, tr.CountryId, tr.StateProvinceId, tr.Zip, tr.TaxCategoryId
                    select tr;
            }, cache => cache.PrepareKeyForShortTermCache(ModelCacheEventConsumer.TAXRATE_ALL_KEY));

            var records = new PagedList<AbcTaxRate>(rez, pageIndex, pageSize);

            return records;
        }

        public virtual async Task<AbcTaxRate> GetTaxRateByIdAsync(int taxRateId)
        {
            return await _abcTaxRateRepository.GetByIdAsync(taxRateId);
        }
        public virtual async Task InsertTaxRateAsync(AbcTaxRate taxRate)
        {
            await _abcTaxRateRepository.InsertAsync(taxRate);
        }
        public virtual async Task UpdateTaxRateAsync(AbcTaxRate taxRate)
        {
            await _abcTaxRateRepository.UpdateAsync(taxRate);
        }

        public async Task<AbcTaxRate> GetAbcTaxRateAsync(
            int storeId,
            int taxCategoryId,
            Address address
        )
        {
            //first, load all tax rate records (cached) - loaded only once
            var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(ModelCacheEventConsumer.ALL_TAX_RATES_MODEL_KEY);
            var allTaxRates = await _staticCacheManager.GetAsync(cacheKey, async () => (await GetAllTaxRatesAsync()).Select(taxRate => new AbcTaxRate
            {
                Id = taxRate.Id,
                StoreId = taxRate.StoreId,
                TaxCategoryId = taxRate.TaxCategoryId,
                CountryId = taxRate.CountryId,
                StateProvinceId = taxRate.StateProvinceId,
                Zip = taxRate.Zip,
                Percentage = taxRate.Percentage,
                IsTaxJarEnabled = taxRate.IsTaxJarEnabled
            }).ToList());

            var countryId = address.CountryId;
            var stateProvinceId = address.StateProvinceId;
            var zip = address.ZipPostalCode?.Trim() ?? string.Empty;

            var existingRates = allTaxRates.Where(taxRate => taxRate.CountryId == countryId && taxRate.TaxCategoryId == taxCategoryId);

            //filter by store
            var matchedByStore = existingRates.Where(taxRate => storeId == taxRate.StoreId || taxRate.StoreId == 0);

            //filter by state/province
            var matchedByStateProvince = matchedByStore.Where(taxRate => stateProvinceId == taxRate.StateProvinceId || taxRate.StateProvinceId == 0);

            //filter by zip
            var matchedByZip = matchedByStateProvince.Where(taxRate => string.IsNullOrWhiteSpace(taxRate.Zip) || taxRate.Zip.Equals(zip, StringComparison.InvariantCultureIgnoreCase));

            //sort from particular to general, more particular cases will be the first
            var foundRecords = matchedByZip.OrderBy(r => r.StoreId == 0).ThenBy(r => r.StateProvinceId == 0).ThenBy(r => string.IsNullOrEmpty(r.Zip));

            return foundRecords.FirstOrDefault();
        }
    }
}