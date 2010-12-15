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
using Nop.Core.Caching;
using Nop.Core.Domain;
using Nop.Data;
using Nop.Core;

namespace Nop.Services
{
    /// <summary>
    /// Provides information about localization
    /// </summary>
    public partial class LocalizationService : ILocalizationService
    {
        #region Constants
        private const string LOCALSTRINGRESOURCES_ALL_KEY = "Nop.localestringresource.all-{0}";
        private const string LOCALSTRINGRESOURCES_PATTERN_KEY = "Nop.localestringresource.";
        #endregion

        #region Fields

        private readonly IRepository<LocaleStringResource> _lsrRespository;
        private readonly ICacheManager _cacheManager;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="lsrRespository">Locale string resource repository</param>
        public LocalizationService(ICacheManager cacheManager,
            IRepository<LocaleStringResource> lsrRespository)
        {
            this._cacheManager = cacheManager;
            this._lsrRespository = lsrRespository;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Deletes a locale string resource
        /// </summary>
        /// <param name="localeStringResource">Locale string resource</param>
        public void DeleteLocaleStringResource(LocaleStringResource localeStringResource)
        {
            if (localeStringResource == null)
                throw new ArgumentNullException("localeStringResource");

            _lsrRespository.Delete(localeStringResource);

            //cache
            _cacheManager.RemoveByPattern(LOCALSTRINGRESOURCES_PATTERN_KEY);
        }

        /// <summary>
        /// Gets a locale string resource
        /// </summary>
        /// <param name="localeStringResourceId">Locale string resource identifier</param>
        /// <returns>Locale string resource</returns>
        public LocaleStringResource GetLocaleStringResourceById(int localeStringResourceId)
        {
            if (localeStringResourceId == 0)
                return null;

            var localeStringResource = _lsrRespository.GetById(localeStringResourceId);

            return localeStringResource;
        }

        /// <summary>
        /// Gets all locale string resources by language identifier
        /// </summary>
        /// <param name="languageId">Language identifier</param>
        /// <returns>Locale string resource collection</returns>
        public Dictionary<string, LocaleStringResource> GetAllResourcesByLanguageId(int languageId)
        {
            string key = string.Format(LOCALSTRINGRESOURCES_ALL_KEY, languageId);
            object obj2 = _cacheManager.Get(key);
            if (obj2 != null)
            {
                return (Dictionary<string, LocaleStringResource>) obj2;
            }
            
            var query = from l in _lsrRespository.Table
                        orderby l.ResourceName
                        where l.LanguageId == languageId
                        select l;
            var localeStringResourceDictionary = query.ToDictionary(s => s.ResourceName.ToLowerInvariant());

            //cache
            _cacheManager.Add(key, localeStringResourceDictionary);

            return localeStringResourceDictionary;
        }

        /// <summary>
        /// Inserts a locale string resource
        /// </summary>
        /// <param name="localeStringResource">Locale string resource</param>
        public void InsertLocaleStringResource(LocaleStringResource localeStringResource)
        {
            if (localeStringResource == null)
                throw new ArgumentNullException("localeStringResource");

            localeStringResource.ResourceName = CommonHelper.EnsureNotNull(localeStringResource.ResourceName);
            localeStringResource.ResourceName = CommonHelper.EnsureMaximumLength(localeStringResource.ResourceName, 200);
            localeStringResource.ResourceValue = CommonHelper.EnsureNotNull(localeStringResource.ResourceValue);


            _lsrRespository.Insert(localeStringResource);

            //cache
            _cacheManager.RemoveByPattern(LOCALSTRINGRESOURCES_PATTERN_KEY);
        }

        /// <summary>
        /// Updates the locale string resource
        /// </summary>
        /// <param name="localeStringResource">Locale string resource</param>
        public void UpdateLocaleStringResource(LocaleStringResource localeStringResource)
        {
            if (localeStringResource == null)
                throw new ArgumentNullException("localeStringResource");

            localeStringResource.ResourceName = CommonHelper.EnsureNotNull(localeStringResource.ResourceName);
            localeStringResource.ResourceName = CommonHelper.EnsureMaximumLength(localeStringResource.ResourceName, 200);
            localeStringResource.ResourceValue = CommonHelper.EnsureNotNull(localeStringResource.ResourceValue);

            _lsrRespository.Update(localeStringResource);

            //cache
            _cacheManager.RemoveByPattern(LOCALSTRINGRESOURCES_PATTERN_KEY);
        }

        #endregion
    }
}
