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
using Nop.Data;
using Nop.Core.Caching;
using Nop.Core.Domain.Localization;
using Nop.Core;

namespace Nop.Services.Localization
{
    /// <summary>
    /// Language service
    /// </summary>
    public partial class LanguageService : ILanguageService
    {
        #region Constants
        private const string LANGUAGES_ALL_KEY = "Nop.language.all-{0}";
        private const string LANGUAGES_BY_ID_KEY = "Nop.language.id-{0}";
        private const string LANGUAGES_PATTERN_KEY = "Nop.language.";
        #endregion

        #region Fields

        private readonly IRepository<Language> _languageRepository;
        private readonly ICacheManager _cacheManager;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="languageRepository">Language repository</param>
        public LanguageService(ICacheManager cacheManager,
            IRepository<Language> languageRepository)
        {
            this._cacheManager = cacheManager;
            this._languageRepository = languageRepository;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Deletes a language
        /// </summary>
        /// <param name="language">Language</param>
        public void DeleteLanguage(Language language)
        {
            if (language == null)
                return;

            _languageRepository.Delete(language);

            //cache
            _cacheManager.RemoveByPattern(LANGUAGES_PATTERN_KEY);
        }

        /// <summary>
        /// Gets all languages
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Language collection</returns>
        public IList<Language> GetAllLanguages(bool showHidden = false)
        {
            string key = string.Format(LANGUAGES_ALL_KEY, showHidden);
            return _cacheManager.Get(key, () =>
            {
                var query = from l in _languageRepository.Table
                            orderby l.DisplayOrder
                            where showHidden || l.Published
                            select l;
                var languages = query.ToList();
                return languages;
            });
        }

        /// <summary>
        /// Gets a language
        /// </summary>
        /// <param name="languageId">Language identifier</param>
        /// <returns>Language</returns>
        public Language GetLanguageById(int languageId)
        {
            if (languageId == 0)
                return null;

            string key = string.Format(LANGUAGES_BY_ID_KEY, languageId);
            return _cacheManager.Get(key, () =>
                                              {
                                                  return _languageRepository.GetById(languageId);
                                              });
        }

        /// <summary>
        /// Inserts a language
        /// </summary>
        /// <param name="language">Language</param>
        public void InsertLanguage(Language language)
        {
            if (language == null)
                throw new ArgumentNullException("language");

            _languageRepository.Insert(language);

            //cache
            _cacheManager.RemoveByPattern(LANGUAGES_PATTERN_KEY);
        }

        /// <summary>
        /// Updates a language
        /// </summary>
        /// <param name="language">Language</param>
        public void UpdateLanguage(Language language)
        {
            if (language == null)
                throw new ArgumentNullException("language");

            _languageRepository.Update(language);

            //cache
            _cacheManager.RemoveByPattern(LANGUAGES_PATTERN_KEY);
        }

        #endregion
    }
}
