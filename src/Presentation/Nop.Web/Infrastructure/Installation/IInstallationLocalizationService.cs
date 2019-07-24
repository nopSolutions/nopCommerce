using System.Collections.Generic;

namespace Nop.Web.Infrastructure.Installation
{
    /// <summary>
    /// Localization service for installation process
    /// </summary>
    public partial interface IInstallationLocalizationService
    {
        /// <summary>
        /// Get locale resource value
        /// </summary>
        /// <param name="resourceName">Resource name</param>
        /// <returns>Resource value</returns>
        string GetResource(string resourceName);

        /// <summary>
        /// Get current language for the installation page
        /// </summary>
        /// <returns>Current language</returns>
        InstallationLanguage GetCurrentLanguage();

        /// <summary>
        /// Save a language for the installation page
        /// </summary>
        /// <param name="languageCode">Language code</param>
        void SaveCurrentLanguage(string languageCode);

        /// <summary>
        /// Get a list of available languages
        /// </summary>
        /// <returns>Available installation languages</returns>
        IList<InstallationLanguage> GetAvailableLanguages();
    }
}
