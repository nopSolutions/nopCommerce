using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;
using Nop.Core;
using Nop.Core.Infrastructure;

namespace Nop.Web.Infrastructure.Installation
{
    /// <summary>
    /// Localization service for installation process
    /// </summary>
    public partial class InstallationLocalizationService : IInstallationLocalizationService
    {
        /// <summary>
        /// Cookie name to language for the installation page
        /// </summary>
        private const string LanguageCookieName = "nop.installation.lang";

        /// <summary>
        /// Available languages
        /// </summary>
        private IList<InstallationLanguage> _availableLanguages;

        /// <summary>
        /// Get locale resource value
        /// </summary>
        /// <param name="resourceName">Resource name</param>
        /// <returns>Resource value</returns>
        public string GetResource(string resourceName)
        {
            var language = GetCurrentLanguage();
            if (language == null)
                return resourceName;
            var resourceValue = language.Resources
                .Where(r => r.Name.Equals(resourceName, StringComparison.InvariantCultureIgnoreCase))
                .Select(r => r.Value)
                .FirstOrDefault();
            if (String.IsNullOrEmpty(resourceValue))
                //return name
                return resourceName;

            return resourceValue;
        }

        /// <summary>
        /// Get current language for the installation page
        /// </summary>
        /// <returns>Current language</returns>
        public virtual InstallationLanguage GetCurrentLanguage()
        {
            var httpContext = EngineContext.Current.Resolve<HttpContextBase>();

            var cookieLanguageCode = "";
            var cookie = httpContext.Request.Cookies[LanguageCookieName];
            if (cookie != null && !String.IsNullOrEmpty(cookie.Value))
                cookieLanguageCode = cookie.Value;

            //ensure it's available (it could be delete since the previous installation)
            var availableLanguages = GetAvailableLanguages();

            var language = availableLanguages
                .FirstOrDefault(l => l.Code.Equals(cookieLanguageCode, StringComparison.InvariantCultureIgnoreCase));
            if (language != null)
                return language;

            //let's find by current browser culture
            if (httpContext.Request.UserLanguages != null)
            {
                var userLanguage = httpContext.Request.UserLanguages.FirstOrDefault();
                if (!String.IsNullOrEmpty(userLanguage))
                {
                    //right. we do "StartsWith" (not "Equals") because we have shorten codes (not full culture names)
                    language = availableLanguages
                        .FirstOrDefault(l => userLanguage.StartsWith(l.Code, StringComparison.InvariantCultureIgnoreCase));
                }
            }
            if (language != null)
                return language;

            //let's return the default one
            language = availableLanguages.FirstOrDefault(l => l.IsDefault);
            if (language != null)
                return language;

            //return any available language
            language = availableLanguages.FirstOrDefault();
            return language;
        }

        /// <summary>
        /// Save a language for the installation page
        /// </summary>
        /// <param name="languageCode">Language code</param>
        public virtual void SaveCurrentLanguage(string languageCode)
        {
            var httpContext = EngineContext.Current.Resolve<HttpContextBase>();

            var cookie = new HttpCookie(LanguageCookieName);
            cookie.HttpOnly = true;
            cookie.Value = languageCode;
            cookie.Expires = DateTime.Now.AddHours(24);
            httpContext.Response.Cookies.Remove(LanguageCookieName);
            httpContext.Response.Cookies.Add(cookie);
        }

        /// <summary>
        /// Get a list of available languages
        /// </summary>
        /// <returns>Available installation languages</returns>
        public virtual IList<InstallationLanguage> GetAvailableLanguages()
        {
            if (_availableLanguages == null)
            {
                _availableLanguages = new List<InstallationLanguage>();
                foreach (var filePath in Directory.EnumerateFiles(CommonHelper.MapPath("~/App_Data/Localization/Installation/"), "*.xml"))
                {
                    var xmlDocument = new XmlDocument();
                    xmlDocument.Load(filePath);


                    //get language code
                    var languageCode = "";
                    //we file name format: installation.{languagecode}.xml
                    var r = new Regex(Regex.Escape("installation.") + "(.*?)" + Regex.Escape(".xml"));
                    var matches = r.Matches(Path.GetFileName(filePath));
                    foreach (Match match in matches)
                        languageCode = match.Groups[1].Value;

                    //get language friendly name
                    var languageName = xmlDocument.SelectSingleNode(@"//Language").Attributes["Name"].InnerText.Trim();

                    //is default
                    var isDefaultAttribute = xmlDocument.SelectSingleNode(@"//Language").Attributes["IsDefault"];
                    var isDefault = isDefaultAttribute != null && Convert.ToBoolean(isDefaultAttribute.InnerText.Trim());

                    //is default
                    var isRightToLeftAttribute = xmlDocument.SelectSingleNode(@"//Language").Attributes["IsRightToLeft"];
                    var isRightToLeft = isRightToLeftAttribute != null && Convert.ToBoolean(isRightToLeftAttribute.InnerText.Trim());

                    //create language
                    var language = new InstallationLanguage
                    {
                        Code = languageCode,
                        Name = languageName,
                        IsDefault = isDefault,
                        IsRightToLeft = isRightToLeft,
                    };
                    //load resources
                    foreach (XmlNode resNode in xmlDocument.SelectNodes(@"//Language/LocaleResource"))
                    {
                        var resNameAttribute = resNode.Attributes["Name"];
                        var resValueNode = resNode.SelectSingleNode("Value");

                        if (resNameAttribute == null)
                            throw new NopException("All installation resources must have an attribute Name=\"Value\".");
                        var resourceName = resNameAttribute.Value.Trim();
                        if (string.IsNullOrEmpty(resourceName))
                            throw new NopException("All installation resource attributes 'Name' must have a value.'");

                        if (resValueNode == null)
                            throw new NopException("All installation resources must have an element \"Value\".");
                        var resourceValue = resValueNode.InnerText.Trim();

                        language.Resources.Add(new InstallationLocaleResource
                        {
                            Name = resourceName,
                            Value = resourceValue
                        });
                    }

                    _availableLanguages.Add(language);
                    _availableLanguages = _availableLanguages.OrderBy(l => l.Name).ToList();

                }
            }
            return _availableLanguages;
        }
    }
}
