//------------------------------------------------------------------------------
// The contents of this file are subject to the nopCommerce Public License Version 1.0 ("License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at  http://www.nopCommerce.com/License.aspx. 
// 
// Software distributed under the License is distributed on an "AS IS" basis, WITHOUT WARRANTY OF ANY KIND, either express or implied. 
// See the License for the specific language governing rights and limitations under the License.
// 
// The Original Code is nopCommerce.
// The Initial Developer of the Original Code is NopSolutions.
// All Rights Reserved.
// 
// Contributor(s): _______. 
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core.Domain;
using Nop.Core;
using Nop.Data;
using Nop.Core.Caching;
using Nop.Core.Domain.Directory;

namespace Nop.Services.Directory
{
    /// <summary>
    /// State province service
    /// </summary>
    public partial class StateProvinceService : IStateProvinceService
    {
        #region Constants
        private const string STATEPROVINCES_ALL_KEY = "Nop.stateprovince.all-{0}";
        private const string STATEPROVINCES_BY_ID_KEY = "Nop.stateprovince.id-{0}";
        private const string STATEPROVINCES_PATTERN_KEY = "Nop.stateprovince.";
        #endregion

        #region Fields

        private readonly IRepository<StateProvince> _stateProvinceRepository;
        private readonly ICacheManager _cacheManager;

        #endregion

        #region Ctor
        
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="stateProvinceRepository">State/province repository</param>
        public StateProvinceService(ICacheManager cacheManager,
            IRepository<StateProvince> stateProvinceRepository)
        {
            this._cacheManager = cacheManager;
            this._stateProvinceRepository = stateProvinceRepository;
        }

        #endregion

        #region Methods
        /// <summary>
        /// Deletes a state/province
        /// </summary>
        /// <param name="stateProvince">The state/province</param>
        public void DeleteStateProvince(StateProvince stateProvince)
        {
            if (stateProvince == null)
                return;

            _stateProvinceRepository.Delete(stateProvince);

            _cacheManager.RemoveByPattern(STATEPROVINCES_PATTERN_KEY);
        }

        /// <summary>
        /// Gets a state/province
        /// </summary>
        /// <param name="stateProvinceId">The state/province identifier</param>
        /// <returns>State/province</returns>
        public StateProvince GetStateProvinceById(int stateProvinceId)
        {
            if (stateProvinceId == 0)
                return null;

            string key = string.Format(STATEPROVINCES_BY_ID_KEY, stateProvinceId);
            return _cacheManager.Get(key, () =>
            {
                var category = _stateProvinceRepository.GetById(stateProvinceId);
                return category;
            });
        }

        /// <summary>
        /// Gets a state/province 
        /// </summary>
        /// <param name="abbreviation">The state/province abbreviation</param>
        /// <returns>State/province</returns>
        public StateProvince GetStateProvinceByAbbreviation(string abbreviation)
        {
            var query = from sp in _stateProvinceRepository.Table
                        where sp.Abbreviation == abbreviation
                        select sp;
            var stateProvince = query.FirstOrDefault();
            return stateProvince;
        }
        
        /// <summary>
        /// Gets a state/province collection by country identifier
        /// </summary>
        /// <param name="countryId">Country identifier</param>
        /// <returns>State/province collection</returns>
        public IList<StateProvince> GetStateProvincesByCountryId(int countryId)
        {
            string key = string.Format(STATEPROVINCES_ALL_KEY, countryId);
            return _cacheManager.Get(key, () =>
            {
                var query = from sp in _stateProvinceRepository.Table
                            orderby sp.DisplayOrder
                            where sp.CountryId == countryId
                            select sp;
                var stateProvinces = query.ToList();
                return stateProvinces;
            });
        }

        /// <summary>
        /// Inserts a state/province
        /// </summary>
        /// <param name="stateProvince">State/province</param>
        public void InsertStateProvince(StateProvince stateProvince)
        {
            if (stateProvince == null)
                throw new ArgumentNullException("stateProvince");

            _stateProvinceRepository.Insert(stateProvince);

            _cacheManager.RemoveByPattern(STATEPROVINCES_PATTERN_KEY);
        }

        /// <summary>
        /// Updates a state/province
        /// </summary>
        /// <param name="stateProvince">State/province</param>
        public void UpdateStateProvince(StateProvince stateProvince)
        {
            if (stateProvince == null)
                throw new ArgumentNullException("stateProvince");

            _stateProvinceRepository.Update(stateProvince);
            
            _cacheManager.RemoveByPattern(STATEPROVINCES_PATTERN_KEY);
        }

        #endregion
    }
}
