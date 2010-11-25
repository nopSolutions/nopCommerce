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
using System.Data.Objects;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using NopSolutions.NopCommerce.BusinessLogic.Audit;
using NopSolutions.NopCommerce.BusinessLogic.Caching;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.Data;
using NopSolutions.NopCommerce.BusinessLogic.Directory;
using NopSolutions.NopCommerce.BusinessLogic.Infrastructure;
using NopSolutions.NopCommerce.Common;
using NopSolutions.NopCommerce.Common.Utils;
 

namespace NopSolutions.NopCommerce.BusinessLogic.Localization
{
    /// <summary>
    /// Provides information about localization
    /// </summary>
    public partial class LocalizationManager : ILocalizationManager
    {
        #region Constants
        private const string LOCALSTRINGRESOURCES_ALL_KEY = "Nop.localestringresource.all-{0}";
        private const string LOCALSTRINGRESOURCES_PATTERN_KEY = "Nop.localestringresource.";
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
        public LocalizationManager(NopObjectContext context)
        {
            this._context = context;
            this._cacheManager = new NopStaticCache();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Deletes a locale string resource
        /// </summary>
        /// <param name="localeStringResourceId">Locale string resource identifier</param>
        public void DeleteLocaleStringResource(int localeStringResourceId)
        {
            var localeStringResource = GetLocaleStringResourceById(localeStringResourceId);
            if (localeStringResource == null)
                return;

            
            if (!_context.IsAttached(localeStringResource))
                _context.LocaleStringResources.Attach(localeStringResource);
            _context.DeleteObject(localeStringResource);
            _context.SaveChanges();
            
            if (this.CacheEnabled)
            {
                _cacheManager.RemoveByPattern(LOCALSTRINGRESOURCES_PATTERN_KEY);
            }
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

            
            var query = from l in _context.LocaleStringResources
                        where l.LocaleStringResourceId == localeStringResourceId
                        select l;
            var localeStringResource = query.SingleOrDefault();

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
            if (this.CacheEnabled && (obj2 != null))
            {
                return (Dictionary<string, LocaleStringResource>)obj2;
            }

            
            var query = from l in _context.LocaleStringResources
                        orderby l.ResourceName
                        where l.LanguageId == languageId
                        select l;
            var localeStringResourceDictionary = query.ToDictionary(s => s.ResourceName.ToLowerInvariant());

            if (this.CacheEnabled)
            {
                _cacheManager.Add(key, localeStringResourceDictionary);
            }
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

            
            
            _context.LocaleStringResources.AddObject(localeStringResource);
            _context.SaveChanges();

            if (this.CacheEnabled)
            {
                _cacheManager.RemoveByPattern(LOCALSTRINGRESOURCES_PATTERN_KEY);
            }
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

            
            if (!_context.IsAttached(localeStringResource))
                _context.LocaleStringResources.Attach(localeStringResource);

            _context.SaveChanges();
            
            if (this.CacheEnabled)
            {
                _cacheManager.RemoveByPattern(LOCALSTRINGRESOURCES_PATTERN_KEY);
            }
        }

        /// <summary>
        /// Gets all locale string resources as XML
        /// </summary>
        /// <param name="languageId">The Language identifier</param>
        /// <returns>XML</returns>
        public string LanguagePackExport(int languageId)
        {
            string xmlPackage = string.Empty;
            
            ObjectParameter xmlPackageParameter = new ObjectParameter("XmlPackage", typeof(string));
            _context.Sp_LanguagePackExport(languageId, xmlPackageParameter);
            xmlPackage = Convert.ToString(xmlPackageParameter.Value);

            return xmlPackage;
        }

        /// <summary>
        /// Inserts all locale string resources from XML
        /// </summary>
        /// <param name="languageId">The Language identifier</param>
        /// <param name="xml">The XML package</param>
        public void LanguagePackImport(int languageId, string xml)
        {
            
            //long-running query
            int? timeout = _context.CommandTimeout;
            try
            {
                _context.CommandTimeout = 600;
                _context.Sp_LanguagePackImport(languageId, xml);
            }
            catch (Exception exc)
            {
                Debug.WriteLine(exc.ToString());
                throw;
            }
            finally
            {
                _context.CommandTimeout = timeout;
            }

            if (this.CacheEnabled)
            {
                _cacheManager.RemoveByPattern(LOCALSTRINGRESOURCES_PATTERN_KEY);
            }
        }

        /// <summary>
        /// Gets a resource string based on the specified ResourceKey property.
        /// </summary>
        /// <param name="resourceKey">A string representing a ResourceKey.</param>
        /// <returns>A string representing the requested resource string.</returns>
        public string GetLocaleResourceString(string resourceKey)
        {
            if (NopContext.Current.WorkingLanguage != null)
            {
                var language = NopContext.Current.WorkingLanguage;
                return GetLocaleResourceString(resourceKey, language.LanguageId);
            }
            else
                return string.Empty;
        }

        /// <summary>
        /// Gets a resource string based on the specified ResourceKey property.
        /// </summary>
        /// <param name="resourceKey">A string representing a ResourceKey.</param>
        /// <param name="languageId">Language identifier</param>
        /// <returns>A string representing the requested resource string.</returns>
        public string GetLocaleResourceString(string resourceKey, int languageId)
        {
            return GetLocaleResourceString(resourceKey, languageId, true);
        }

        /// <summary>
        /// Gets a resource string based on the specified ResourceKey property.
        /// </summary>
        /// <param name="resourceKey">A string representing a ResourceKey.</param>
        /// <param name="languageId">Language identifier</param>
        /// <param name="logIfNotFound">A value indicating whether to log error if locale string resource is not found</param>
        /// <returns>A string representing the requested resource string.</returns>
        public string GetLocaleResourceString(string resourceKey,
            int languageId, bool logIfNotFound)
        {
            return GetLocaleResourceString(resourceKey, languageId, logIfNotFound, string.Empty);
        }

        /// <summary>
        /// Gets a resource string based on the specified ResourceKey property.
        /// </summary>
        /// <param name="resourceKey">A string representing a ResourceKey.</param>
        /// <param name="languageId">Language identifier</param>
        /// <param name="logIfNotFound">A value indicating whether to log error if locale string resource is not found</param>
        /// <param name="defaultValue">Default value</param>
        /// <returns>A string representing the requested resource string.</returns>
        public string GetLocaleResourceString(string resourceKey, int languageId,
            bool logIfNotFound, string defaultValue)
        {
            string result = string.Empty;
            if (resourceKey == null)
                resourceKey = string.Empty;
            resourceKey = resourceKey.Trim().ToLowerInvariant();
            var resources = GetAllResourcesByLanguageId(languageId);

            if (resources.ContainsKey(resourceKey))
            {
                var lsr = resources[resourceKey];
                if (lsr != null)
                    result = lsr.ResourceValue;
            }
            if (String.IsNullOrEmpty(result))
            {
                if (logIfNotFound)
                {
                    IoC.Resolve<ILogService>().InsertLog(LogTypeEnum.CommonError, "Resource string is not found", string.Format("Resource string ({0}) is not found. Language Id ={1}", resourceKey, languageId));
                }

                if (!String.IsNullOrEmpty(defaultValue))
                {
                    result = defaultValue;
                }
                else
                {
                    result = resourceKey;
                }
            }
            return result;
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
                return IoC.Resolve<ISettingManager>().GetSettingValueBoolean("Cache.LocaleStringResourceManager.CacheEnabled");
            }
        }

        /// <summary>
        /// Gets or sets the default admin language
        /// </summary>
        public Language DefaultAdminLanguage
        {
            get
            {
                int defaultAdminLanguageId = IoC.Resolve<ISettingManager>().GetSettingValueInteger("Localization.DefaultAdminLanguageId");

                var language = IoC.Resolve<ILanguageService>().GetLanguageById(defaultAdminLanguageId);
                if (language != null && language.Published)
                {
                    return language;
                }
                else
                {
                    var publishedLanguages = IoC.Resolve<ILanguageService>().GetAllLanguages(false);
                    foreach (Language publishedLanguage in publishedLanguages)
                        return publishedLanguage;
                }
                throw new NopException("Default admin language could not be loaded");
            }
            set
            {
                if (value != null)
                    IoC.Resolve<ISettingManager>().SetParam("Localization.DefaultAdminLanguageId", value.LanguageId.ToString());
            }
        }
        #endregion
    }
}
