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
    /// Provides information about localizable entities
    /// </summary>
    public partial class LocalizedEntityService : ILocalizedEntityService
    {
        #region Fields

        private readonly IRepository<LocalizedProperty> _localizedPropertyRespository;
        private readonly ICacheManager _cacheManager;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="localizedPropertyRespository">Localized property repository</param>
        public LocalizedEntityService(ICacheManager cacheManager,
            IRepository<LocalizedProperty> localizedPropertyRespository)
        {
            this._cacheManager = cacheManager;
            this._localizedPropertyRespository = localizedPropertyRespository;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Deletes a localized property
        /// </summary>
        /// <param name="localizedProperty">Localized property</param>
        public void DeleteLocalizedProperty(LocalizedProperty localizedProperty)
        {
            if (localizedProperty == null)
                throw new ArgumentNullException("localizedProperty");

            _localizedPropertyRespository.Delete(localizedProperty);
        }

        /// <summary>
        /// Gets a localized property
        /// </summary>
        /// <param name="localizedPropertyId">Localized property identifier</param>
        /// <returns>Localized property</returns>
        public LocalizedProperty GetLocalizedPropertyById(int localizedPropertyId)
        {
            if (localizedPropertyId == 0)
                return null;

            var localizedProperty = _localizedPropertyRespository.GetById(localizedPropertyId);
            return localizedProperty;
        }

        /// <summary>
        /// Gets localized properties
        /// </summary>
        /// <param name="entityId">Entity identifier</param>
        /// <param name="languageId">Language identifier</param>
        /// <param name="localeKeyGroup">Locale key group</param>
        /// <returns>Localized properties</returns>
        public Dictionary<string, LocalizedProperty> GetLocalizedProperties(int entityId, int languageId, string localeKeyGroup)
        {
            if (entityId == 0 || languageId == 0)
                return new Dictionary<string, LocalizedProperty>();

            var query = from lp in _localizedPropertyRespository.Table
                        orderby lp.LocaleKey
                        where lp.LanguageId == languageId &&
                        lp.EntityId == entityId &&
                        lp.LocaleKeyGroup == localeKeyGroup
                        select lp;
            var lpDictionary = query.ToDictionary(s => s.LocaleKey);
            return lpDictionary;
        }

        /// <summary>
        /// Inserts a localized property
        /// </summary>
        /// <param name="localizedProperty">Localized property</param>
        public void InsertLocalizedProperty(LocalizedProperty localizedProperty)
        {
            if (localizedProperty == null)
                throw new ArgumentNullException("localizedProperty");

            _localizedPropertyRespository.Insert(localizedProperty);
        }

        /// <summary>
        /// Updates the localized property
        /// </summary>
        /// <param name="localizedProperty">Localized property</param>
        public void UpdateLocalizedProperty(LocalizedProperty localizedProperty)
        {
            if (localizedProperty == null)
                throw new ArgumentNullException("localizedProperty");

            _localizedPropertyRespository.Update(localizedProperty);
        }


        #endregion
    }
}