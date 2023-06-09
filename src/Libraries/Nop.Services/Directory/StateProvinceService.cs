using Nop.Core.Caching;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Directory;
using Nop.Data;
using Nop.Services.Localization;

namespace Nop.Services.Directory
{
    /// <summary>
    /// State province service
    /// </summary>
    public partial class StateProvinceService : IStateProvinceService
    {
        #region Fields

        protected readonly IStaticCacheManager _staticCacheManager;
        protected readonly ILocalizationService _localizationService;
        protected readonly IRepository<StateProvince> _stateProvinceRepository;

        #endregion

        #region Ctor

        public StateProvinceService(IStaticCacheManager staticCacheManager,
            ILocalizationService localizationService,
            IRepository<StateProvince> stateProvinceRepository)
        {
            _staticCacheManager = staticCacheManager;
            _localizationService = localizationService;
            _stateProvinceRepository = stateProvinceRepository;
        }

        #endregion

        #region Methods
        /// <summary>
        /// Deletes a state/province
        /// </summary>
        /// <param name="stateProvince">The state/province</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeleteStateProvinceAsync(StateProvince stateProvince)
        {
            await _stateProvinceRepository.DeleteAsync(stateProvince);
        }

        /// <summary>
        /// Gets a state/province
        /// </summary>
        /// <param name="stateProvinceId">The state/province identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the state/province
        /// </returns>
        public virtual async Task<StateProvince> GetStateProvinceByIdAsync(int stateProvinceId)
        {
            return await _stateProvinceRepository.GetByIdAsync(stateProvinceId, cache => default);
        }

        /// <summary>
        /// Gets a state/province by abbreviation
        /// </summary>
        /// <param name="abbreviation">The state/province abbreviation</param>
        /// <param name="countryId">Country identifier; pass null to load the state regardless of a country</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the state/province
        /// </returns>
        public virtual async Task<StateProvince> GetStateProvinceByAbbreviationAsync(string abbreviation, int? countryId = null)
        {
            if (string.IsNullOrEmpty(abbreviation))
                return null;

            var key = _staticCacheManager.PrepareKeyForDefaultCache(NopDirectoryDefaults.StateProvincesByAbbreviationCacheKey
                , abbreviation, countryId ?? 0);

            var query = _stateProvinceRepository.Table.Where(state => state.Abbreviation == abbreviation);

            //filter by country
            if (countryId.HasValue)
                query = query.Where(state => state.CountryId == countryId);

            return await _staticCacheManager.GetAsync(key, async () => await query.FirstOrDefaultAsync());
        }

        /// <summary>
        /// Gets a state/province by address 
        /// </summary>
        /// <param name="address">Address</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the country
        /// </returns>
        public virtual async Task<StateProvince> GetStateProvinceByAddressAsync(Address address)
        {
            return await GetStateProvinceByIdAsync(address?.StateProvinceId ?? 0);
        }

        /// <summary>
        /// Gets a state/province collection by country identifier
        /// </summary>
        /// <param name="countryId">Country identifier</param>
        /// <param name="languageId">Language identifier. It's used to sort states by localized names (if specified); pass 0 to skip it</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the states
        /// </returns>
        public virtual async Task<IList<StateProvince>> GetStateProvincesByCountryIdAsync(int countryId, int languageId = 0, bool showHidden = false)
        {
            var key = _staticCacheManager.PrepareKeyForDefaultCache(NopDirectoryDefaults.StateProvincesByCountryCacheKey, countryId, languageId, showHidden);

            return await _staticCacheManager.GetAsync(key, async () =>
            {
                var query = from sp in _stateProvinceRepository.Table
                            orderby sp.DisplayOrder, sp.Name
                            where sp.CountryId == countryId &&
                            (showHidden || sp.Published)
                            select sp;
                var stateProvinces = await query.ToListAsync();

                if (languageId > 0)
                    //we should sort states by localized names when they have the same display order
                    stateProvinces = await stateProvinces.ToAsyncEnumerable()
                        .OrderBy(c => c.DisplayOrder)
                        .ThenByAwait(async c => await _localizationService.GetLocalizedAsync(c, x => x.Name, languageId))
                        .ToListAsync();

                return stateProvinces;
            });
        }

        /// <summary>
        /// Gets all states/provinces
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the states
        /// </returns>
        public virtual async Task<IList<StateProvince>> GetStateProvincesAsync(bool showHidden = false)
        {
            var query = from sp in _stateProvinceRepository.Table
                        orderby sp.CountryId, sp.DisplayOrder, sp.Name
                        where showHidden || sp.Published
                        select sp;


            var stateProvinces = await _staticCacheManager.GetAsync(_staticCacheManager.PrepareKeyForDefaultCache(NopDirectoryDefaults.StateProvincesAllCacheKey, showHidden), async () => await query.ToListAsync());

            return stateProvinces;
        }

        /// <summary>
        /// Inserts a state/province
        /// </summary>
        /// <param name="stateProvince">State/province</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task InsertStateProvinceAsync(StateProvince stateProvince)
        {
            await _stateProvinceRepository.InsertAsync(stateProvince);
        }

        /// <summary>
        /// Updates a state/province
        /// </summary>
        /// <param name="stateProvince">State/province</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task UpdateStateProvinceAsync(StateProvince stateProvince)
        {
            await _stateProvinceRepository.UpdateAsync(stateProvince);
        }

        #endregion
    }
}