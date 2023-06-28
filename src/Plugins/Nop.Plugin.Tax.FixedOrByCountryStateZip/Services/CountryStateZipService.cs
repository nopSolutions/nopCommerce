using Nop.Core;
using Nop.Data;
using Nop.Plugin.Tax.FixedOrByCountryStateZip.Domain;
using Nop.Plugin.Tax.FixedOrByCountryStateZip.Infrastructure.Cache;

namespace Nop.Plugin.Tax.FixedOrByCountryStateZip.Services
{
    /// <summary>
    /// Tax rate service
    /// </summary>
    public class CountryStateZipService : ICountryStateZipService
    {
        #region Fields

        protected readonly IRepository<TaxRate> _taxRateRepository;

        #endregion

        #region Ctor

        public CountryStateZipService(IRepository<TaxRate> taxRateRepository)
        {
            _taxRateRepository = taxRateRepository;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Deletes a tax rate
        /// </summary>
        /// <param name="taxRate">Tax rate</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeleteTaxRateAsync(TaxRate taxRate)
        {
            await _taxRateRepository.DeleteAsync(taxRate);
        }

        /// <summary>
        /// Gets all tax rates
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the ax rates
        /// </returns>
        public virtual async Task<IPagedList<TaxRate>> GetAllTaxRatesAsync(int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var rez = await _taxRateRepository.GetAllAsync(query =>
            {
                return from tr in query
                       orderby tr.StoreId, tr.CountryId, tr.StateProvinceId, tr.Zip, tr.TaxCategoryId
                       select tr;
            }, cache => cache.PrepareKeyForShortTermCache(ModelCacheEventConsumer.TAXRATE_ALL_KEY));

            var records = new PagedList<TaxRate>(rez, pageIndex, pageSize);

            return records;
        }

        /// <summary>
        /// Gets a tax rate
        /// </summary>
        /// <param name="taxRateId">Tax rate identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the ax rate
        /// </returns>
        public virtual async Task<TaxRate> GetTaxRateByIdAsync(int taxRateId)
        {
            return await _taxRateRepository.GetByIdAsync(taxRateId);
        }

        /// <summary>
        /// Inserts a tax rate
        /// </summary>
        /// <param name="taxRate">Tax rate</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task InsertTaxRateAsync(TaxRate taxRate)
        {
            await _taxRateRepository.InsertAsync(taxRate);
        }

        /// <summary>
        /// Updates the tax rate
        /// </summary>
        /// <param name="taxRate">Tax rate</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task UpdateTaxRateAsync(TaxRate taxRate)
        {
            await _taxRateRepository.UpdateAsync(taxRate);
        }

        #endregion
    }
}