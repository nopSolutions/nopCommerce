using System.Collections.Generic;

namespace Nop.Web.Framework.Themes
{
    /// <summary>
    /// Represents a theme provider
    /// </summary>
    public partial interface IThemeProvider
    {
        /// <summary>
        /// Get all theme descriptors
        /// </summary>
        /// <returns>List of the theme descriptor</returns>
        IList<ThemeDescriptor> GetThemeDescriptors();

        /// <summary>
        /// Get theme descriptor by theme system name
        /// </summary>
        /// <param name="systemName">Theme system name</param>
        /// <returns>Theme descriptor</returns>
        ThemeDescriptor GetThemeDescriptorBySystemName(string systemName);

        /// <summary>
        /// Check whether theme descriptor with specified system name exists
        /// </summary>
        /// <param name="systemName">Theme system name</param>
        /// <returns>True if theme descriptor exists; otherwise false</returns>
        bool ThemeDescriptorExists(string systemName);
    }
}
