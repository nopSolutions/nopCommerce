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
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Data.Objects;
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
    /// Language manager
    /// </summary>
    public partial class LanguageManager
    {
        #region Constants
        private const string LANGUAGES_ALL_KEY = "Nop.language.all-{0}";
        private const string LANGUAGES_BY_ID_KEY = "Nop.language.id-{0}";
        private const string LANGUAGES_PATTERN_KEY = "Nop.language.";
        #endregion

        #region Methods
        /// <summary>
        /// Deletes a language
        /// </summary>
        /// <param name="languageId">Language identifier</param>
        public static void DeleteLanguage(int languageId)
        {
            var language = GetLanguageById(languageId);
            if (language == null)
                return;

            var context = ObjectContextHelper.CurrentObjectContext;
            if (!context.IsAttached(language))
                context.Languages.Attach(language);
            context.DeleteObject(language);
            context.SaveChanges();

            if (LanguageManager.CacheEnabled)
            {
                NopStaticCache.RemoveByPattern(LANGUAGES_PATTERN_KEY);
            }
        }

        /// <summary>
        /// Gets all languages
        /// </summary>
        /// <returns>Language collection</returns>
        public static List<Language> GetAllLanguages()
        {
            bool showHidden = NopContext.Current.IsAdmin;
            return (List<Language>)GetAllLanguages(showHidden);
        }

        /// <summary>
        /// Gets all languages
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Language collection</returns>
        public static List<Language> GetAllLanguages(bool showHidden)
        {
            string key = string.Format(LANGUAGES_ALL_KEY, showHidden);
            object obj2 = NopStaticCache.Get(key);
            if (LanguageManager.CacheEnabled && (obj2 != null))
            {
                return (List<Language>)obj2;
            }

            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from l in context.Languages
                        orderby l.DisplayOrder
                        where showHidden || l.Published
                        select l;
            var languages = query.ToList();

            if (LanguageManager.CacheEnabled)
            {
                NopStaticCache.Max(key, languages);
            }
            return languages;
        }

        /// <summary>
        /// Gets a language
        /// </summary>
        /// <param name="languageId">Language identifier</param>
        /// <returns>Language</returns>
        public static Language GetLanguageById(int languageId)
        {
            if (languageId == 0)
                return null;

            string key = string.Format(LANGUAGES_BY_ID_KEY, languageId);
            object obj2 = NopStaticCache.Get(key);
            if (LanguageManager.CacheEnabled && (obj2 != null))
            {
                return (Language)obj2;
            }

            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from l in context.Languages
                        where l.LanguageId == languageId
                        select l;
            var language = query.SingleOrDefault();

            if (LanguageManager.CacheEnabled)
            {
                NopStaticCache.Max(key, language);
            }
            return language;
        }

        /// <summary>
        /// Inserts a language
        /// </summary>
        /// <param name="name">The name</param>
        /// <param name="languageCulture">The language culture</param>
        /// <param name="flagImageFileName">The flag image file name</param>
        /// <param name="published">A value indicating whether the language is published</param>
        /// <param name="displayOrder">The display order</param>
        /// <returns>Language</returns>
        public static Language InsertLanguage(string name, string languageCulture,
            string flagImageFileName, bool published, int displayOrder)
        {
            name = CommonHelper.EnsureMaximumLength(name, 100);
            languageCulture = CommonHelper.EnsureMaximumLength(languageCulture, 20);
            flagImageFileName = CommonHelper.EnsureMaximumLength(flagImageFileName, 50);

            var context = ObjectContextHelper.CurrentObjectContext;

            var language = context.Languages.CreateObject();
            language.Name = name;
            language.LanguageCulture = languageCulture;
            language.FlagImageFileName = flagImageFileName;
            language.Published = published;
            language.DisplayOrder = displayOrder;

            context.Languages.AddObject(language);
            context.SaveChanges();

            if (LanguageManager.CacheEnabled)
            {
                NopStaticCache.RemoveByPattern(LANGUAGES_PATTERN_KEY);
            }
            return language;
        }

        /// <summary>
        /// Updates a language
        /// </summary>
        /// <param name="languageId">Language identifier</param>
        /// <param name="name">The name</param>
        /// <param name="languageCulture">The language culture</param>
        /// <param name="flagImageFileName">The flag image file name</param>
        /// <param name="published">A value indicating whether the language is published</param>
        /// <param name="displayOrder">The display order</param>
        /// <returns>Language</returns>
        public static Language UpdateLanguage(int languageId,
            string name, string languageCulture,
            string flagImageFileName, bool published, int displayOrder)
        {
            name = CommonHelper.EnsureMaximumLength(name, 100);
            languageCulture = CommonHelper.EnsureMaximumLength(languageCulture, 20);
            flagImageFileName = CommonHelper.EnsureMaximumLength(flagImageFileName, 50);

            var language = GetLanguageById(languageId);
            if (language == null)
                return null;

            var context = ObjectContextHelper.CurrentObjectContext;
            if (!context.IsAttached(language))
                context.Languages.Attach(language);

            language.Name = name;
            language.LanguageCulture = languageCulture;
            language.FlagImageFileName = flagImageFileName;
            language.Published = published;
            language.DisplayOrder = displayOrder;
            context.SaveChanges();

            if (LanguageManager.CacheEnabled)
            {
                NopStaticCache.RemoveByPattern(LANGUAGES_PATTERN_KEY);
            }
            return language;
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
                return SettingManager.GetSettingValueBoolean("Cache.LanguageManager.CacheEnabled");
            }
        }
        #endregion
    }
}
