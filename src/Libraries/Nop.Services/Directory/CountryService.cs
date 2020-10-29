using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Stores;
using Nop.Data;
using Nop.Services.Localization;

namespace Nop.Services.Directory
{
    /// <summary>
    /// Country service
    /// </summary>
    public partial class CountryService : ICountryService
    {
        #region Fields

        private readonly CatalogSettings _catalogSettings;
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly ILocalizationService _localizationService;
        private readonly IRepository<Country> _countryRepository;
        private readonly IRepository<StoreMapping> _storeMappingRepository;
        private readonly IStoreContext _storeContext;

        #endregion

        #region Ctor

        public CountryService(CatalogSettings catalogSettings,
            IStaticCacheManager staticCacheManager,
            ILocalizationService localizationService,
            IRepository<Country> countryRepository,
            IRepository<StoreMapping> storeMappingRepository,
            IStoreContext storeContext)
        {
            _catalogSettings = catalogSettings;
            _staticCacheManager = staticCacheManager;
            _localizationService = localizationService;
            _countryRepository = countryRepository;
            _storeMappingRepository = storeMappingRepository;
            _storeContext = storeContext;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Deletes a country
        /// </summary>
        /// <param name="country">Country</param>
        public virtual async Task DeleteCountryAsync(Country country)
        {
            await _countryRepository.DeleteAsync(country);
        }

        /// <summary>
        /// Gets all countries
        /// </summary>
        /// <param name="languageId">Language identifier. It's used to sort countries by localized names (if specified); pass 0 to skip it</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Countries</returns>
        public virtual async Task<IList<Country>> GetAllCountriesAsync(int languageId = 0, bool showHidden = false)
        {
            var key = _staticCacheManager.PrepareKeyForDefaultCache(NopDirectoryDefaults.CountriesAllCacheKey, languageId,
                showHidden, await _storeContext.GetCurrentStoreAsync());

            return await _staticCacheManager.GetAsync(key, async () =>
            {
                var countries = await _countryRepository.GetAllAsync(query =>
                {
                    if (!showHidden)
                        query = query.Where(c => c.Published);

                    if (!showHidden && !_catalogSettings.IgnoreStoreLimitations)
                    {
                        //Store mapping
                        var currentStoreId = _storeContext.GetCurrentStoreAsync().Result.Id;
                        query = from c in query
                            join sc in _storeMappingRepository.Table
                                on new {c1 = c.Id, c2 = nameof(Country)} equals new
                                {
                                    c1 = sc.EntityId, c2 = sc.EntityName
                                } into c_sc
                            from sc in c_sc.DefaultIfEmpty()
                            where !c.LimitedToStores || currentStoreId == sc.StoreId
                            select c;

                        query = query.Distinct();
                    }

                    return query.OrderBy(c => c.DisplayOrder).ThenBy(c => c.Name);
                });

                if (languageId > 0)
                {
                    //we should sort countries by localized names when they have the same display order
                    countries = countries
                        .OrderBy(c => c.DisplayOrder)
                        .ThenBy(c => _localizationService.GetLocalizedAsync(c, x => x.Name, languageId).Result)
                        .ToList();
                }

                return countries;
            });
        }

        /// <summary>
        /// Gets all countries that allow billing
        /// </summary>
        /// <param name="languageId">Language identifier. It's used to sort countries by localized names (if specified); pass 0 to skip it</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Countries</returns>
        public virtual async Task<IList<Country>> GetAllCountriesForBillingAsync(int languageId = 0, bool showHidden = false)
        {
            return (await GetAllCountriesAsync(languageId, showHidden)).Where(c => c.AllowsBilling).ToList();
        }

        /// <summary>
        /// Gets all countries that allow shipping
        /// </summary>
        /// <param name="languageId">Language identifier. It's used to sort countries by localized names (if specified); pass 0 to skip it</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Countries</returns>
        public virtual async Task<IList<Country>> GetAllCountriesForShippingAsync(int languageId = 0, bool showHidden = false)
        {
            return (await GetAllCountriesAsync(languageId, showHidden)).Where(c => c.AllowsShipping).ToList();
        }

        /// <summary>
        /// Gets a country by address 
        /// </summary>
        /// <param name="address">Address</param>
        /// <returns>Country</returns>
        public virtual async Task<Country> GetCountryByAddressAsync(Address address)
        {
            return await GetCountryByIdAsync(address?.CountryId ?? 0);
        }

        /// <summary>
        /// Gets a country 
        /// </summary>
        /// <param name="countryId">Country identifier</param>
        /// <returns>Country</returns>
        public virtual async Task<Country> GetCountryByIdAsync(int countryId)
        {
            return await _countryRepository.GetByIdAsync(countryId, cache => default);
        }

        /// <summary>
        /// Get countries by identifiers
        /// </summary>
        /// <param name="countryIds">Country identifiers</param>
        /// <returns>Countries</returns>
        public virtual async Task<IList<Country>> GetCountriesByIdsAsync(int[] countryIds)
        {
            return await _countryRepository.GetByIdsAsync(countryIds);
        }

        /// <summary>
        /// Gets a country by two letter ISO code
        /// </summary>
        /// <param name="twoLetterIsoCode">Country two letter ISO code</param>
        /// <returns>Country</returns>
        public virtual async Task<Country> GetCountryByTwoLetterIsoCodeAsync(string twoLetterIsoCode)
        {
            if (string.IsNullOrEmpty(twoLetterIsoCode))
                return null;

            var key = _staticCacheManager.PrepareKeyForDefaultCache(NopDirectoryDefaults.CountriesByTwoLetterCodeCacheKey, twoLetterIsoCode);

            var query = from c in _countryRepository.Table
                where c.TwoLetterIsoCode == twoLetterIsoCode
                select c;

            return await _staticCacheManager.GetAsync(key, async () => await query.ToAsyncEnumerable().FirstOrDefaultAsync());
        }

        /// <summary>
        /// Gets a country by three letter ISO code
        /// </summary>
        /// <param name="threeLetterIsoCode">Country three letter ISO code</param>
        /// <returns>Country</returns>
        public virtual async Task<Country> GetCountryByThreeLetterIsoCodeAsync(string threeLetterIsoCode)
        {
            if (string.IsNullOrEmpty(threeLetterIsoCode))
                return null;

            var key = _staticCacheManager.PrepareKeyForDefaultCache(NopDirectoryDefaults.CountriesByThreeLetterCodeCacheKey, threeLetterIsoCode);

            var query = from c in _countryRepository.Table
                where c.ThreeLetterIsoCode == threeLetterIsoCode
                select c;

            return await _staticCacheManager.GetAsync(key, async () => await query.ToAsyncEnumerable().FirstOrDefaultAsync());
        }

        /// <summary>
        /// Inserts a country
        /// </summary>
        /// <param name="country">Country</param>
        public virtual async Task InsertCountryAsync(Country country)
        {
            await _countryRepository.InsertAsync(country);
        }

        /// <summary>
        /// Updates the country
        /// </summary>
        /// <param name="country">Country</param>
        public virtual async Task UpdateCountryAsync(Country country)
        {
            await _countryRepository.UpdateAsync(country);
        }

        #endregion
    }
}