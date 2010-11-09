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
using System.Globalization;
using NopSolutions.NopCommerce.BusinessLogic.Audit;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.Directory;
using NopSolutions.NopCommerce.Common;
using System.Collections.Generic;
using NopSolutions.NopCommerce.BusinessLogic.IoC;
 

namespace NopSolutions.NopCommerce.BusinessLogic.Localization
{
    /// <summary>
    /// Provides information about localization
    /// </summary>
    public partial class LocalizationManager
    {
        #region Methods
        /// <summary>
        /// Gets currency string
        /// </summary>
        /// <param name="amount">Amount</param>
        /// <returns>Currency string without exchange rate</returns>
        public static string GetCurrencyString(decimal amount)
        {
            bool showCurrency = true;
            var targetCurrency = NopContext.Current.WorkingCurrency;
            return GetCurrencyString(amount, showCurrency, targetCurrency);
        }

        /// <summary>
        /// Gets currency string
        /// </summary>
        /// <param name="amount">Amount</param>
        /// <param name="showCurrency">A value indicating whether to show a currency</param>
        /// <param name="targetCurrency">Target currency</param>
        /// <returns>Currency string without exchange rate</returns>
        public static string GetCurrencyString(decimal amount, 
            bool showCurrency, Currency targetCurrency)
        {
            string result = string.Empty;
            if (!String.IsNullOrEmpty(targetCurrency.CustomFormatting))
            {
                result = amount.ToString(targetCurrency.CustomFormatting);
            }
            else
            {
                if (!String.IsNullOrEmpty(targetCurrency.DisplayLocale))
                {
                    result = amount.ToString("C", new CultureInfo(targetCurrency.DisplayLocale));
                }
                else
                {
                    result = String.Format("{0} ({1})", amount.ToString("N"), targetCurrency.CurrencyCode);
                    return result;
                }
            }

            if (showCurrency && IoCFactory.Resolve<ICurrencyService>().GetAllCurrencies().Count > 1)
                result = String.Format("{0} ({1})", result, targetCurrency.CurrencyCode);
            return result;
        }

        /// <summary>
        /// Gets a resource string based on the specified ResourceKey property.
        /// </summary>
        /// <param name="resourceKey">A string representing a ResourceKey.</param>
        /// <returns>A string representing the requested resource string.</returns>
        public static string GetLocaleResourceString(string resourceKey)
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
        public static string GetLocaleResourceString(string resourceKey, int languageId)
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
        public static string GetLocaleResourceString(string resourceKey,
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
        public static string GetLocaleResourceString(string resourceKey, int languageId,
            bool logIfNotFound, string defaultValue)
        {
            string result = string.Empty;
            if (resourceKey == null)
                resourceKey = string.Empty;
            resourceKey = resourceKey.Trim().ToLowerInvariant();
            var resources = IoCFactory.Resolve<ILocaleStringResourceManager>().GetAllResourcesByLanguageId(languageId);

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
                    IoCFactory.Resolve<ILogService>().InsertLog(LogTypeEnum.CommonError, "Resource string is not found", string.Format("Resource string ({0}) is not found. Language Id ={1}", resourceKey, languageId));
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

        /// <summary>
        /// Imports language pack from XML
        /// </summary>
        /// <param name="languageId">Language identifier</param>
        /// <param name="content">XML content</param>
        public static void LanguagePackImport(int languageId, string content)
        {
            IoCFactory.Resolve<ILocaleStringResourceManager>().InsertAllLocaleStringResourcesFromXml(languageId, content);
        }

        /// <summary>
        /// Exports language pack to XML
        /// </summary>
        /// <param name="languageId">Language identifier</param>
        /// <returns>XML content</returns>
        public static string LanguagePackExport(int languageId)
        {
            return IoCFactory.Resolve<ILocaleStringResourceManager>().GetAllLocaleStringResourcesAsXml(languageId);
        }
        
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the default admin language
        /// </summary>
        public static Language DefaultAdminLanguage
        {
            get
            {
                int defaultAdminLanguageId = IoCFactory.Resolve<ISettingManager>().GetSettingValueInteger("Localization.DefaultAdminLanguageId");

                var language = IoCFactory.Resolve<ILanguageService>().GetLanguageById(defaultAdminLanguageId);
                if (language != null && language.Published)
                {
                    return language;
                }
                else
                {
                    var publishedLanguages = IoCFactory.Resolve<ILanguageService>().GetAllLanguages(false);
                    foreach (Language publishedLanguage in publishedLanguages)
                        return publishedLanguage;
                }
                throw new NopException("Default admin language could not be loaded");
            }
            set
            {
                if (value != null)
                    IoCFactory.Resolve<ISettingManager>().SetParam("Localization.DefaultAdminLanguageId", value.LanguageId.ToString());
            }
        }
        
        #endregion
    }
}
