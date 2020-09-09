using System;
using System.Linq;
using System.Threading.Tasks;
using LinqToDB;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Data;
using Nop.Plugin.Tax.FixedOrByCountryStateZip.Domain;
using Nop.Plugin.Tax.FixedOrByCountryStateZip.Infrastructure.Cache;
using Nop.Services.Caching;
using Nop.Services.Events;

namespace Nop.Plugin.Tax.FixedOrByCountryStateZip.Services
{
    /// <summary>
    /// Tax rate service
    /// </summary>
    public partial class CountryStateZipService : ICountryStateZipService
    {
        #region Fields

        private readonly ICacheKeyService _cacheKeyService;
        private readonly IEventPublisher _eventPublisher;
        private readonly IRepository<TaxRate> _taxRateRepository;
        private readonly IStaticCacheManager _staticCacheManager;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cacheKeyService">Cache key service</param>
        /// <param name="eventPublisher">Event publisher</param>
        /// <param name="staticCacheManager">Cache manager</param>
        /// <param name="taxRateRepository">Tax rate repository</param>
        public CountryStateZipService(ICacheKeyService cacheKeyService,
            IEventPublisher eventPublisher,            
            IRepository<TaxRate> taxRateRepository,
            IStaticCacheManager staticCacheManager)
        {
            _cacheKeyService = cacheKeyService;
            _eventPublisher = eventPublisher;            
            _taxRateRepository = taxRateRepository;
            _staticCacheManager = staticCacheManager;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Deletes a tax rate
        /// </summary>
        /// <param name="taxRate">Tax rate</param>
        public virtual async Task DeleteTaxRate(TaxRate taxRate)
        {
            if (taxRate == null)
                throw new ArgumentNullException(nameof(taxRate));

            await _taxRateRepository.Delete(taxRate);

            //event notification
            await _eventPublisher.EntityDeleted(taxRate);
        }

        /// <summary>
        /// Gets all tax rates
        /// </summary>
        /// <returns>Tax rates</returns>
        public virtual async Task<IPagedList<TaxRate>> GetAllTaxRates(int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var key = _cacheKeyService.PrepareKeyForShortTermCache(ModelCacheEventConsumer.TAXRATE_ALL_KEY);
            var rez = await _staticCacheManager.Get(key, async () =>
            {
                var query = from tr in _taxRateRepository.Table
                            orderby tr.StoreId, tr.CountryId, tr.StateProvinceId, tr.Zip, tr.TaxCategoryId
                            select tr;

                return await query.ToListAsync();
            });

            var records = new PagedList<TaxRate>(rez, pageIndex, pageSize);

            return records;
        }

        /// <summary>
        /// Gets a tax rate
        /// </summary>
        /// <param name="taxRateId">Tax rate identifier</param>
        /// <returns>Tax rate</returns>
        public virtual async Task<TaxRate> GetTaxRateById(int taxRateId)
        {
            if (taxRateId == 0)
                return null;

            return await _taxRateRepository.GetById(taxRateId);
        }

        /// <summary>
        /// Inserts a tax rate
        /// </summary>
        /// <param name="taxRate">Tax rate</param>
        public virtual async Task InsertTaxRate(TaxRate taxRate)
        {
            if (taxRate == null)
                throw new ArgumentNullException(nameof(taxRate));

            await _taxRateRepository.Insert(taxRate);

            //event notification
            await _eventPublisher.EntityInserted(taxRate);
        }

        /// <summary>
        /// Updates the tax rate
        /// </summary>
        /// <param name="taxRate">Tax rate</param>
        public virtual async Task UpdateTaxRate(TaxRate taxRate)
        {
            if (taxRate == null)
                throw new ArgumentNullException(nameof(taxRate));

            await _taxRateRepository.Update(taxRate);

            //event notification
            await _eventPublisher.EntityUpdated(taxRate);
        }

        #endregion
    }
}