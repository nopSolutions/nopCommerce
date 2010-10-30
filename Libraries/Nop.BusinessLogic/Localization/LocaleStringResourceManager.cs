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
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml;
using NopSolutions.NopCommerce.BusinessLogic.Caching;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.Data;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.BusinessLogic.IoC;
using System.Data.Objects;

namespace NopSolutions.NopCommerce.BusinessLogic.Localization
{
    /// <summary>
    /// Locale string resource manager
    /// </summary>
    public partial class LocaleStringResourceManager : ILocaleStringResourceManager
    {
        #region Constants
        private const string LOCALSTRINGRESOURCES_ALL_KEY = "Nop.localestringresource.all-{0}";
        private const string LOCALSTRINGRESOURCES_PATTERN_KEY = "Nop.localestringresource.";
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
        public LocaleStringResourceManager(NopObjectContext context)
        {
            _context = context;
            _cacheManager = new NopStaticCache();
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
        public string GetAllLocaleStringResourcesAsXml(int languageId)
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
        public void InsertAllLocaleStringResourcesFromXml(int languageId, string xml)
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

        #endregion
        
        #region Properties

        /// <summary>
        /// Gets a value indicating whether cache is enabled
        /// </summary>
        public bool CacheEnabled
        {
            get
            {
                return IoCFactory.Resolve<ISettingManager>().GetSettingValueBoolean("Cache.LocaleStringResourceManager.CacheEnabled");
            }
        }

        #endregion
    }
}
