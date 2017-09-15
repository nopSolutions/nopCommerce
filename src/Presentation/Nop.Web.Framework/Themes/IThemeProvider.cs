using System.Collections.Generic;

namespace Nop.Web.Framework.Themes
{
    /// <summary>
    /// Represents a theme provider
    /// </summary>
    public partial interface IThemeProvider
    {
        /// <summary>
        /// Get all theme configurations
        /// </summary>
        /// <returns>List of the theme configuration</returns>
        IList<ThemeConfiguration> GetThemeConfigurations();

        /// <summary>
        /// Get theme configuration by theme system name
        /// </summary>
        /// <param name="systemName">Theme system name</param>
        /// <returns>Theme configuration</returns>
        ThemeConfiguration GetThemeConfigurationBySystemName(string systemName);

        /// <summary>
        /// Check whether theme configuration with specified system name exists
        /// </summary>
        /// <param name="systemName">Theme system name</param>
        /// <returns>True if theme configuration exists; otherwise false</returns>
        bool ThemeConfigurationExists(string systemName);
    }
}
