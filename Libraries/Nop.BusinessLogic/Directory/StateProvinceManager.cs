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

namespace NopSolutions.NopCommerce.BusinessLogic.Directory
{
    /// <summary>
    /// State province manager
    /// </summary>
    public partial class StateProvinceManager
    {
        #region Constants
        private const string STATEPROVINCES_ALL_KEY = "Nop.stateprovince.all-{0}";
        private const string STATEPROVINCES_BY_ID_KEY = "Nop.stateprovince.id-{0}";
        private const string STATEPROVINCES_PATTERN_KEY = "Nop.stateprovince.";
        #endregion
        
        #region Methods
        /// <summary>
        /// Deletes a state/province
        /// </summary>
        /// <param name="stateProvinceId">The state/province identifier</param>
        public static void DeleteStateProvince(int stateProvinceId)
        {
            var stateProvince = GetStateProvinceById(stateProvinceId);
            if (stateProvince == null)
                return;

            var context = ObjectContextHelper.CurrentObjectContext;
            if (!context.IsAttached(stateProvince))
                context.StateProvinces.Attach(stateProvince);
            context.DeleteObject(stateProvince);
            context.SaveChanges();

            if (StateProvinceManager.CacheEnabled)
            {
                NopRequestCache.RemoveByPattern(STATEPROVINCES_PATTERN_KEY);
            }
        }

        /// <summary>
        /// Gets a state/province
        /// </summary>
        /// <param name="stateProvinceId">The state/province identifier</param>
        /// <returns>State/province</returns>
        public static StateProvince GetStateProvinceById(int stateProvinceId)
        {
            if (stateProvinceId == 0)
                return null;

            string key = string.Format(STATEPROVINCES_BY_ID_KEY, stateProvinceId);
            object obj2 = NopRequestCache.Get(key);
            if (StateProvinceManager.CacheEnabled && (obj2 != null))
            {
                return (StateProvince)obj2;
            }

            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from sp in context.StateProvinces
                        where sp.StateProvinceId == stateProvinceId
                        select sp;
            var stateProvince = query.SingleOrDefault();
            
            if (StateProvinceManager.CacheEnabled)
            {
                NopRequestCache.Add(key, stateProvince);
            }
            return stateProvince;
        }

        /// <summary>
        /// Gets a state/province 
        /// </summary>
        /// <param name="abbreviation">The state/province abbreviation</param>
        /// <returns>State/province</returns>
        public static StateProvince GetStateProvinceByAbbreviation(string abbreviation)
        {
            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from sp in context.StateProvinces
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
        public static List<StateProvince> GetStateProvincesByCountryId(int countryId)
        {
            string key = string.Format(STATEPROVINCES_ALL_KEY, countryId);
            object obj2 = NopRequestCache.Get(key);
            if (StateProvinceManager.CacheEnabled && (obj2 != null))
            {
                return (List<StateProvince>)obj2;
            }

            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from sp in context.StateProvinces
                        orderby sp.DisplayOrder
                        where sp.CountryId == countryId
                        select sp;
            var stateProvinceCollection = query.ToList();
            
            if (StateProvinceManager.CacheEnabled)
            {
                NopRequestCache.Add(key, stateProvinceCollection);
            }
            return stateProvinceCollection;
        }

        /// <summary>
        /// Inserts a state/province
        /// </summary>
        /// <param name="countryId">The country identifier</param>
        /// <param name="name">The name</param>
        /// <param name="abbreviation">The abbreviation</param>
        /// <param name="displayOrder">The display order</param>
        /// <returns>State/province</returns>
        public static StateProvince InsertStateProvince(int countryId,
            string name, string abbreviation, int displayOrder)
        {
            name = CommonHelper.EnsureMaximumLength(name, 100);
            abbreviation = CommonHelper.EnsureMaximumLength(abbreviation, 30);

            var context = ObjectContextHelper.CurrentObjectContext;

            var stateProvince = context.StateProvinces.CreateObject();
            stateProvince.CountryId = countryId;
            stateProvince.Name = name;
            stateProvince.Abbreviation = abbreviation;
            stateProvince.DisplayOrder = displayOrder;

            context.StateProvinces.AddObject(stateProvince);
            context.SaveChanges();
            
            if (StateProvinceManager.CacheEnabled)
            {
                NopRequestCache.RemoveByPattern(STATEPROVINCES_PATTERN_KEY);
            }

            return stateProvince;
        }

        /// <summary>
        /// Updates a state/province
        /// </summary>
        /// <param name="stateProvinceId">The state/province identifier</param>
        /// <param name="countryId">The country identifier</param>
        /// <param name="name">The name</param>
        /// <param name="abbreviation">The abbreviation</param>
        /// <param name="displayOrder">The display order</param>
        /// <returns>State/province</returns>
        public static StateProvince UpdateStateProvince(int stateProvinceId,
            int countryId, string name, string abbreviation, int displayOrder)
        {
            name = CommonHelper.EnsureMaximumLength(name, 100);
            abbreviation = CommonHelper.EnsureMaximumLength(abbreviation, 30);

            var stateProvince = GetStateProvinceById(stateProvinceId);
            if (stateProvince == null)
                return null;

            var context = ObjectContextHelper.CurrentObjectContext;
            if (!context.IsAttached(stateProvince))
                context.StateProvinces.Attach(stateProvince);

            stateProvince.CountryId = countryId;
            stateProvince.Name = name;
            stateProvince.Abbreviation = abbreviation;
            stateProvince.DisplayOrder = displayOrder;
            context.SaveChanges();

            if (StateProvinceManager.CacheEnabled)
            {
                NopRequestCache.RemoveByPattern(STATEPROVINCES_PATTERN_KEY);
            }

            return stateProvince;
        }
        #endregion
        
        #region Properties
        /// <summary>
        /// Gets a value indicating whether cache is enabled
        /// </summary>
        public static bool CacheEnabled
        {
            get
            {
                return SettingManager.GetSettingValueBoolean("Cache.StateProvinceManager.CacheEnabled");
            }
        }
        #endregion
    }
}
