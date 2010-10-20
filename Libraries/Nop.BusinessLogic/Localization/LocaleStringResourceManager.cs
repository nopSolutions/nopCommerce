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

namespace NopSolutions.NopCommerce.BusinessLogic.Localization
{
    /// <summary>
    /// Locale string resource manager
    /// </summary>
    public partial class LocaleStringResourceManager
    {
        #region Constants
        private const string LOCALSTRINGRESOURCES_ALL_KEY = "Nop.localestringresource.all-{0}";
        private const string LOCALSTRINGRESOURCES_PATTERN_KEY = "Nop.localestringresource.";
        #endregion
        
        #region Methods
        /// <summary>
        /// Deletes a locale string resource
        /// </summary>
        /// <param name="localeStringResourceId">Locale string resource identifier</param>
        public static void DeleteLocaleStringResource(int localeStringResourceId)
        {
            var localeStringResource = GetLocaleStringResourceById(localeStringResourceId);
            if (localeStringResource == null)
                return;

            var context = ObjectContextHelper.CurrentObjectContext;
            if (!context.IsAttached(localeStringResource))
                context.LocaleStringResources.Attach(localeStringResource);
            context.DeleteObject(localeStringResource);
            context.SaveChanges();
            
            if (LocaleStringResourceManager.CacheEnabled)
            {
                NopStaticCache.RemoveByPattern(LOCALSTRINGRESOURCES_PATTERN_KEY);
            }
        }

        /// <summary>
        /// Gets a locale string resource
        /// </summary>
        /// <param name="localeStringResourceId">Locale string resource identifier</param>
        /// <returns>Locale string resource</returns>
        public static LocaleStringResource GetLocaleStringResourceById(int localeStringResourceId)
        {
            if (localeStringResourceId == 0)
                return null;

            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from l in context.LocaleStringResources
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
        public static Dictionary<string, LocaleStringResource> GetAllResourcesByLanguageId(int languageId)
        {
            string key = string.Format(LOCALSTRINGRESOURCES_ALL_KEY, languageId);
            object obj2 = NopStaticCache.Get(key);
            if (LocaleStringResourceManager.CacheEnabled && (obj2 != null))
            {
                return (Dictionary<string, LocaleStringResource>)obj2;
            }

            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from l in context.LocaleStringResources
                        orderby l.ResourceName
                        where l.LanguageId == languageId
                        select l;
            var localeStringResourceDictionary = query.ToDictionary(s => s.ResourceName.ToLowerInvariant());

            if (LocaleStringResourceManager.CacheEnabled)
            {
                NopStaticCache.Max(key, localeStringResourceDictionary);
            }
            return localeStringResourceDictionary;
        }

        /// <summary>
        /// Inserts a locale string resource
        /// </summary>
        /// <param name="localeStringResource">Locale string resource</param>
        public static void InsertLocaleStringResource(LocaleStringResource localeStringResource)
        {
            if (localeStringResource == null)
                throw new ArgumentNullException("localeStringResource");

            localeStringResource.ResourceName = CommonHelper.EnsureNotNull(localeStringResource.ResourceName);
            localeStringResource.ResourceName = CommonHelper.EnsureMaximumLength(localeStringResource.ResourceName, 200);
            localeStringResource.ResourceValue = CommonHelper.EnsureNotNull(localeStringResource.ResourceValue);

            var context = ObjectContextHelper.CurrentObjectContext;
            
            context.LocaleStringResources.AddObject(localeStringResource);
            context.SaveChanges();

            if (LocaleStringResourceManager.CacheEnabled)
            {
                NopStaticCache.RemoveByPattern(LOCALSTRINGRESOURCES_PATTERN_KEY);
            }
        }

        /// <summary>
        /// Updates the locale string resource
        /// </summary>
        /// <param name="localeStringResource">Locale string resource</param>
        public static void UpdateLocaleStringResource(LocaleStringResource localeStringResource)
        {
            if (localeStringResource == null)
                throw new ArgumentNullException("localeStringResource");

            localeStringResource.ResourceName = CommonHelper.EnsureNotNull(localeStringResource.ResourceName);
            localeStringResource.ResourceName = CommonHelper.EnsureMaximumLength(localeStringResource.ResourceName, 200);
            localeStringResource.ResourceValue = CommonHelper.EnsureNotNull(localeStringResource.ResourceValue);

            var context = ObjectContextHelper.CurrentObjectContext;
            if (!context.IsAttached(localeStringResource))
                context.LocaleStringResources.Attach(localeStringResource);

            context.SaveChanges();
            
            if (LocaleStringResourceManager.CacheEnabled)
            {
                NopStaticCache.RemoveByPattern(LOCALSTRINGRESOURCES_PATTERN_KEY);
            }
        }

        /// <summary>
        /// Gets all locale string resources as XML
        /// </summary>
        /// <param name="languageId">The Language identifier</param>
        /// <returns>XML</returns>
        public static string GetAllLocaleStringResourcesAsXml(int languageId)
        {
            string xmlPackage = string.Empty;
            var context = ObjectContextHelper.CurrentObjectContext;
            context.Sp_LanguagePackExport(languageId, out xmlPackage);

            return xmlPackage;
        }

        /// <summary>
        /// Inserts all locale string resources from XML
        /// </summary>
        /// <param name="languageId">The Language identifier</param>
        /// <param name="xml">The XML package</param>
        public static void InsertAllLocaleStringResourcesFromXml(int languageId, string xml)
        {
            var context = ObjectContextHelper.CurrentObjectContext;
            //long-running query
            int? timeout = context.CommandTimeout;
            try
            {
                context.CommandTimeout = 600;
                context.Sp_LanguagePackImport(languageId, xml);
            }
            catch (Exception exc)
            {
                Debug.WriteLine(exc.ToString());
                throw;
            }
            finally
            {
                context.CommandTimeout = timeout;
            }

            if (LocaleStringResourceManager.CacheEnabled)
            {
                NopStaticCache.RemoveByPattern(LOCALSTRINGRESOURCES_PATTERN_KEY);
            }
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
                return SettingManager.GetSettingValueBoolean("Cache.LocaleStringResourceManager.CacheEnabled");
            }
        }
        #endregion
    }
}
