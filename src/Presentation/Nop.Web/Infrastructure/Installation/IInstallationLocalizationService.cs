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
        /// Get current browser culture
        /// </summary>
        /// <returns>Current culture</returns>
        string GetBrowserCulture();

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

        /// <summary>
        /// Get a list of available data provider types
        /// </summary>
        /// <param name="valuesToExclude">Values to exclude</param>
        /// <param name="useLocalization">Localize</param>
        /// <returns>SelectList</returns>
        Dictionary<int, string> GetAvailableProviderTypes(int[] valuesToExclude = null, bool useLocalization = true);
    }
}
