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
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the list of the theme descriptor
        /// </returns>
        Task<IList<ThemeDescriptor>> GetThemesAsync();

        /// <summary>
        /// Get a theme by the system name
        /// </summary>
        /// <param name="systemName">Theme system name</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the me descriptor
        /// </returns>
        Task<ThemeDescriptor> GetThemeBySystemNameAsync(string systemName);

        /// <summary>
        /// Check whether the theme with specified system name exists
        /// </summary>
        /// <param name="systemName">Theme system name</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the rue if the theme exists; otherwise false
        /// </returns>
        Task<bool> ThemeExistsAsync(string systemName);
    }
}