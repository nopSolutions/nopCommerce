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
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Text;
using NopSolutions.NopCommerce.BusinessLogic.Caching;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.Data;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.BusinessLogic.IoC;

namespace NopSolutions.NopCommerce.BusinessLogic.Directory
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

        /// <summary>
        /// Object context
        /// </summary>
        protected NopObjectContext _context;

        /// <summary>
        /// Cache manager
        /// </summary>
        protected ICacheManager _cacheManager;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="context">Object context</param>
        public StateProvinceService(NopObjectContext context)
        {
            _context = context;
            _cacheManager = new NopRequestCache();
        }

        #endregion

        #region Methods
        /// <summary>
        /// Deletes a state/province
        /// </summary>
        /// <param name="stateProvinceId">The state/province identifier</param>
        public void DeleteStateProvince(int stateProvinceId)
        {
            var stateProvince = GetStateProvinceById(stateProvinceId);
            if (stateProvince == null)
                return;

            
            if (!_context.IsAttached(stateProvince))
                _context.StateProvinces.Attach(stateProvince);
            _context.DeleteObject(stateProvince);
            _context.SaveChanges();

            if (this.CacheEnabled)
            {
                _cacheManager.RemoveByPattern(STATEPROVINCES_PATTERN_KEY);
            }
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
            object obj2 = _cacheManager.Get(key);
            if (this.CacheEnabled && (obj2 != null))
            {
                return (StateProvince)obj2;
            }

            
            var query = from sp in _context.StateProvinces
                        where sp.StateProvinceId == stateProvinceId
                        select sp;
            var stateProvince = query.SingleOrDefault();
            
            if (this.CacheEnabled)
            {
                _cacheManager.Add(key, stateProvince);
            }
            return stateProvince;
        }

        /// <summary>
        /// Gets a state/province 
        /// </summary>
        /// <param name="abbreviation">The state/province abbreviation</param>
        /// <returns>State/province</returns>
        public StateProvince GetStateProvinceByAbbreviation(string abbreviation)
        {
            
            var query = from sp in _context.StateProvinces
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
        public List<StateProvince> GetStateProvincesByCountryId(int countryId)
        {
            string key = string.Format(STATEPROVINCES_ALL_KEY, countryId);
            object obj2 = _cacheManager.Get(key);
            if (this.CacheEnabled && (obj2 != null))
            {
                return (List<StateProvince>)obj2;
            }

            
            var query = from sp in _context.StateProvinces
                        orderby sp.DisplayOrder
                        where sp.CountryId == countryId
                        select sp;
            var stateProvinceCollection = query.ToList();
            
            if (this.CacheEnabled)
            {
                _cacheManager.Add(key, stateProvinceCollection);
            }
            return stateProvinceCollection;
        }

        /// <summary>
        /// Inserts a state/province
        /// </summary>
        /// <param name="stateProvince">State/province</param>
        public void InsertStateProvince(StateProvince stateProvince)
        {
            if (stateProvince == null)
                throw new ArgumentNullException("stateProvince");

            stateProvince.Name = CommonHelper.EnsureNotNull(stateProvince.Name);
            stateProvince.Name = CommonHelper.EnsureMaximumLength(stateProvince.Name, 100);
            stateProvince.Abbreviation = CommonHelper.EnsureNotNull(stateProvince.Abbreviation);
            stateProvince.Abbreviation = CommonHelper.EnsureMaximumLength(stateProvince.Abbreviation, 30);
            
            
            
            _context.StateProvinces.AddObject(stateProvince);
            _context.SaveChanges();
            
            if (this.CacheEnabled)
            {
                _cacheManager.RemoveByPattern(STATEPROVINCES_PATTERN_KEY);
            }
        }

        /// <summary>
        /// Updates a state/province
        /// </summary>
        /// <param name="stateProvince">State/province</param>
        public void UpdateStateProvince(StateProvince stateProvince)
        {
            if (stateProvince == null)
                throw new ArgumentNullException("stateProvince");

            stateProvince.Name = CommonHelper.EnsureNotNull(stateProvince.Name);
            stateProvince.Name = CommonHelper.EnsureMaximumLength(stateProvince.Name, 100);
            stateProvince.Abbreviation = CommonHelper.EnsureNotNull(stateProvince.Abbreviation);
            stateProvince.Abbreviation = CommonHelper.EnsureMaximumLength(stateProvince.Abbreviation, 30);

            
            if (!_context.IsAttached(stateProvince))
                _context.StateProvinces.Attach(stateProvince);

            _context.SaveChanges();

            if (this.CacheEnabled)
            {
                _cacheManager.RemoveByPattern(STATEPROVINCES_PATTERN_KEY);
            }
        }
        #endregion
        
        #region Properties
        /// <summary>
        /// Gets a value indicating whether cache is enabled
        /// </summary>
        public bool CacheEnabled
        {
            get
            {
                return IoCFactory.Resolve<ISettingManager>().GetSettingValueBoolean("Cache.StateProvinceManager.CacheEnabled");
            }
        }
        #endregion
    }
}
