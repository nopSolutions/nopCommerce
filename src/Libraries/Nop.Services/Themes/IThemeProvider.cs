using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nop.Services.Themes
{
    /// <summary>
    /// Represents a theme provider
    /// </summary>
    public partial interface IThemeProvider
    {
        /// <summary>
        /// Get theme descriptor from the description text
        /// </summary>
        /// <param name="text">Description text</param>
        /// <returns>Theme descriptor</returns>
        ThemeDescriptor GetThemeDescriptorFromText(string text);

        /// <summary>
        /// Get all themes
        /// </summary>
        /// <returns>List of the theme descriptor</returns>
        Task<IList<ThemeDescriptor>> GetThemesAsync();

        /// <summary>
        /// Get a theme by the system name
        /// </summary>
        /// <param name="systemName">Theme system name</param>
        /// <returns>Theme descriptor</returns>
        Task<ThemeDescriptor> GetThemeBySystemNameAsync(string systemName);

        /// <summary>
        /// Check whether the theme with specified system name exists
        /// </summary>
        /// <param name="systemName">Theme system name</param>
        /// <returns>True if the theme exists; otherwise false</returns>
        Task<bool> ThemeExistsAsync(string systemName);
    }
}