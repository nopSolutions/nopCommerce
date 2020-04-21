using System;
using System.Linq;
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
        public virtual void DeleteTaxRate(TaxRate taxRate)
        {
            if (taxRate == null)
                throw new ArgumentNullException(nameof(taxRate));

            _taxRateRepository.Delete(taxRate);

            //event notification
            _eventPublisher.EntityDeleted(taxRate);
        }

        /// <summary>
        /// Gets all tax rates
        /// </summary>
        /// <returns>Tax rates</returns>
        public virtual IPagedList<TaxRate> GetAllTaxRates(int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var key = _cacheKeyService.PrepareKeyForShortTermCache(ModelCacheEventConsumer.TAXRATE_ALL_KEY);
            var rez = _staticCacheManager.Get(key, () =>
            {
                var query = from tr in _taxRateRepository.Table
                            orderby tr.StoreId, tr.CountryId, tr.StateProvinceId, tr.Zip, tr.TaxCategoryId
                            select tr;

                return query.ToList();
            });

            var records = new PagedList<TaxRate>(rez, pageIndex, pageSize);

            return records;
        }

        /// <summary>
        /// Gets a tax rate
        /// </summary>
        /// <param name="taxRateId">Tax rate identifier</param>
        /// <returns>Tax rate</returns>
        public virtual TaxRate GetTaxRateById(int taxRateId)
        {
            if (taxRateId == 0)
                return null;

            return _taxRateRepository.GetById(taxRateId);
        }

        /// <summary>
        /// Inserts a tax rate
        /// </summary>
        /// <param name="taxRate">Tax rate</param>
        public virtual void InsertTaxRate(TaxRate taxRate)
        {
            if (taxRate == null)
                throw new ArgumentNullException(nameof(taxRate));

            _taxRateRepository.Insert(taxRate);

            //event notification
            _eventPublisher.EntityInserted(taxRate);
        }

        /// <summary>
        /// Updates the tax rate
        /// </summary>
        /// <param name="taxRate">Tax rate</param>
        public virtual void UpdateTaxRate(TaxRate taxRate)
        {
            if (taxRate == null)
                throw new ArgumentNullException(nameof(taxRate));

            _taxRateRepository.Update(taxRate);

            //event notification
            _eventPublisher.EntityUpdated(taxRate);
        }

        #endregion
    }
}