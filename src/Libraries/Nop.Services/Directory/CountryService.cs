using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Directory;
using Nop.Data;
using Nop.Services.Localization;
using Nop.Services.Stores;

namespace Nop.Services.Directory
{
    /// <summary>
    /// Country service
    /// </summary>
    public partial class CountryService : ICountryService
    {
        #region Fields

        protected IStaticCacheManager StaticCacheManager { get; }
        protected ILocalizationService LocalizationService { get; }
        protected IRepository<Country> CountryRepository { get; }
        protected IStoreContext StoreContext { get; }
        protected IStoreMappingService StoreMappingService { get; }

        #endregion

        #region Ctor

        public CountryService(
            IStaticCacheManager staticCacheManager,
            ILocalizationService localizationService,
            IRepository<Country> countryRepository,
            IStoreContext storeContext,
            IStoreMappingService storeMappingService)
        {
            StaticCacheManager = staticCacheManager;
            LocalizationService = localizationService;
            CountryRepository = countryRepository;
            StoreContext = storeContext;
            StoreMappingService = storeMappingService;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Deletes a country
        /// </summary>
        /// <param name="country">Country</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeleteCountryAsync(Country country)
        {
            await CountryRepository.DeleteAsync(country);
        }

        /// <summary>
        /// Gets all countries
        /// </summary>
        /// <param name="languageId">Language identifier. It's used to sort countries by localized names (if specified); pass 0 to skip it</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the countries
        /// </returns>
        public virtual async Task<IList<Country>> GetAllCountriesAsync(int languageId = 0, bool showHidden = false)
        {
            var store = await StoreContext.GetCurrentStoreAsync();
            var key = StaticCacheManager.PrepareKeyForDefaultCache(NopDirectoryDefaults.CountriesAllCacheKey, languageId,
                showHidden, store);

            return await StaticCacheManager.GetAsync(key, async () =>
            {
                var countries = await CountryRepository.GetAllAsync(async query =>
                {
                    if (!showHidden)
                        query = query.Where(c => c.Published);

                    //apply store mapping constraints
                    if (!showHidden)
                        query = await StoreMappingService.ApplyStoreMapping(query, store.Id);

                    return query.OrderBy(c => c.DisplayOrder).ThenBy(c => c.Name);
                });

                if (languageId > 0)
                {
                    //we should sort countries by localized names when they have the same display order
                    countries = await countries
                        .ToAsyncEnumerable()
                        .OrderBy(c => c.DisplayOrder)
                        .ThenByAwait(async c => await LocalizationService.GetLocalizedAsync(c, x => x.Name, languageId))
                        .ToListAsync();
                }

                return countries;
            });
        }

        /// <summary>
        /// Gets all countries that allow billing
        /// </summary>
        /// <param name="languageId">Language identifier. It's used to sort countries by localized names (if specified); pass 0 to skip it</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the countries
        /// </returns>
        public virtual async Task<IList<Country>> GetAllCountriesForBillingAsync(int languageId = 0, bool showHidden = false)
        {
            return (await GetAllCountriesAsync(languageId, showHidden)).Where(c => c.AllowsBilling).ToList();
        }

        /// <summary>
        /// Gets all countries that allow shipping
        /// </summary>
        /// <param name="languageId">Language identifier. It's used to sort countries by localized names (if specified); pass 0 to skip it</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the countries
        /// </returns>
        public virtual async Task<IList<Country>> GetAllCountriesForShippingAsync(int languageId = 0, bool showHidden = false)
        {
            return (await GetAllCountriesAsync(languageId, showHidden)).Where(c => c.AllowsShipping).ToList();
        }

        /// <summary>
        /// Gets a country by address 
        /// </summary>
        /// <param name="address">Address</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the country
        /// </returns>
        public virtual async Task<Country> GetCountryByAddressAsync(Address address)
        {
            return await GetCountryByIdAsync(address?.CountryId ?? 0);
        }

        /// <summary>
        /// Gets a country 
        /// </summary>
        /// <param name="countryId">Country identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the country
        /// </returns>
        public virtual async Task<Country> GetCountryByIdAsync(int countryId)
        {
            return await CountryRepository.GetByIdAsync(countryId, cache => default);
        }

        /// <summary>
        /// Get countries by identifiers
        /// </summary>
        /// <param name="countryIds">Country identifiers</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the countries
        /// </returns>
        public virtual async Task<IList<Country>> GetCountriesByIdsAsync(int[] countryIds)
        {
            return await CountryRepository.GetByIdsAsync(countryIds);
        }

        /// <summary>
        /// Gets a country by two letter ISO code
        /// </summary>
        /// <param name="twoLetterIsoCode">Country two letter ISO code</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the country
        /// </returns>
        public virtual async Task<Country> GetCountryByTwoLetterIsoCodeAsync(string twoLetterIsoCode)
        {
            if (string.IsNullOrEmpty(twoLetterIsoCode))
                return null;

            var key = StaticCacheManager.PrepareKeyForDefaultCache(NopDirectoryDefaults.CountriesByTwoLetterCodeCacheKey, twoLetterIsoCode);

            var query = from c in CountryRepository.Table
                        where c.TwoLetterIsoCode == twoLetterIsoCode
                        select c;

            return await StaticCacheManager.GetAsync(key, async () => await query.FirstOrDefaultAsync());
        }

        /// <summary>
        /// Gets a country by three letter ISO code
        /// </summary>
        /// <param name="threeLetterIsoCode">Country three letter ISO code</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the country
        /// </returns>
        public virtual async Task<Country> GetCountryByThreeLetterIsoCodeAsync(string threeLetterIsoCode)
        {
            if (string.IsNullOrEmpty(threeLetterIsoCode))
                return null;

            var key = StaticCacheManager.PrepareKeyForDefaultCache(NopDirectoryDefaults.CountriesByThreeLetterCodeCacheKey, threeLetterIsoCode);

            var query = from c in CountryRepository.Table
                        where c.ThreeLetterIsoCode == threeLetterIsoCode
                        select c;

            return await StaticCacheManager.GetAsync(key, async () => await query.FirstOrDefaultAsync());
        }

        /// <summary>
        /// Inserts a country
        /// </summary>
        /// <param name="country">Country</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task InsertCountryAsync(Country country)
        {
            await CountryRepository.InsertAsync(country);
        }

        /// <summary>
        /// Updates the country
        /// </summary>
        /// <param name="country">Country</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task UpdateCountryAsync(Country country)
        {
            await CountryRepository.UpdateAsync(country);
        }

        #endregion
    }
}