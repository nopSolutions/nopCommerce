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
using NopSolutions.NopCommerce.BusinessLogic.Caching;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.Data;
using NopSolutions.NopCommerce.BusinessLogic.Infrastructure;
using NopSolutions.NopCommerce.Common.Utils;

namespace NopSolutions.NopCommerce.BusinessLogic.Directory
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

        /// <summary>
        /// Object context
        /// </summary>
        private readonly NopObjectContext _context;

        /// <summary>
        /// Cache manager
        /// </summary>
        private readonly ICacheManager _cacheManager;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="context">Object context</param>
        public LanguageService(NopObjectContext context)
        {
            this._context = context;
            this._cacheManager = new NopRequestCache();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Deletes a language
        /// </summary>
        /// <param name="languageId">Language identifier</param>
        public void DeleteLanguage(int languageId)
        {
            var language = GetLanguageById(languageId);
            if (language == null)
                return;

            
            if (!_context.IsAttached(language))
                _context.Languages.Attach(language);
            _context.DeleteObject(language);
            _context.SaveChanges();

            if (this.CacheEnabled)
            {
                _cacheManager.RemoveByPattern(LANGUAGES_PATTERN_KEY);
            }
        }

        /// <summary>
        /// Gets all languages
        /// </summary>
        /// <returns>Language collection</returns>
        public List<Language> GetAllLanguages()
        {
            bool showHidden = NopContext.Current.IsAdmin;
            return GetAllLanguages(showHidden);
        }

        /// <summary>
        /// Gets all languages
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Language collection</returns>
        public List<Language> GetAllLanguages(bool showHidden)
        {
            string key = string.Format(LANGUAGES_ALL_KEY, showHidden);
            object obj2 = _cacheManager.Get(key);
            if (this.CacheEnabled && (obj2 != null))
            {
                return (List<Language>)obj2;
            }

            
            var query = from l in _context.Languages
                        orderby l.DisplayOrder
                        where showHidden || l.Published
                        select l;
            var languages = query.ToList();

            if (this.CacheEnabled)
            {
                _cacheManager.Add(key, languages);
            }
            return languages;
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
            object obj2 = _cacheManager.Get(key);
            if (this.CacheEnabled && (obj2 != null))
            {
                return (Language)obj2;
            }

            
            var query = from l in _context.Languages
                        where l.LanguageId == languageId
                        select l;
            var language = query.SingleOrDefault();

            if (this.CacheEnabled)
            {
                _cacheManager.Add(key, language);
            }
            return language;
        }

        /// <summary>
        /// Inserts a language
        /// </summary>
        /// <param name="language">Language</param>
        public void InsertLanguage(Language language)
        {
            if (language == null)
                throw new ArgumentNullException("language");
            
            language.Name = CommonHelper.EnsureNotNull(language.Name);
            language.Name = CommonHelper.EnsureMaximumLength(language.Name, 100);
            language.LanguageCulture = CommonHelper.EnsureNotNull(language.LanguageCulture);
            language.LanguageCulture = CommonHelper.EnsureMaximumLength(language.LanguageCulture, 20);
            language.FlagImageFileName = CommonHelper.EnsureNotNull(language.FlagImageFileName);
            language.FlagImageFileName = CommonHelper.EnsureMaximumLength(language.FlagImageFileName, 50);

            

            _context.Languages.AddObject(language);
            _context.SaveChanges();

            if (this.CacheEnabled)
            {
                _cacheManager.RemoveByPattern(LANGUAGES_PATTERN_KEY);
            }
        }

        /// <summary>
        /// Updates a language
        /// </summary>
        /// <param name="language">Language</param>
        public void UpdateLanguage(Language language)
        {
            if (language == null)
                throw new ArgumentNullException("language");

            language.Name = CommonHelper.EnsureNotNull(language.Name);
            language.Name = CommonHelper.EnsureMaximumLength(language.Name, 100);
            language.LanguageCulture = CommonHelper.EnsureNotNull(language.LanguageCulture);
            language.LanguageCulture = CommonHelper.EnsureMaximumLength(language.LanguageCulture, 20);
            language.FlagImageFileName = CommonHelper.EnsureNotNull(language.FlagImageFileName);
            language.FlagImageFileName = CommonHelper.EnsureMaximumLength(language.FlagImageFileName, 50);

            
            if (!_context.IsAttached(language))
                _context.Languages.Attach(language);

            _context.SaveChanges();

            if (this.CacheEnabled)
            {
                _cacheManager.RemoveByPattern(LANGUAGES_PATTERN_KEY);
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
                return IoC.Resolve<ISettingManager>().GetSettingValueBoolean("Cache.LanguageManager.CacheEnabled");
            }
        }

        #endregion
    }
}
