namespace Nop.Web.Framework.Themes
{
    /// <summary>
    /// Work context
    /// </summary>
    public interface IThemeContext
    {
        /// <summary>
        /// Get or set current theme for desktops
        /// </summary>
        string WorkingDesktopTheme { get; set; }

        /// <summary>
        /// Get current theme for mobile (e.g. Mobile)
        /// </summary>
        string WorkingMobileTheme { get; }
    }
}
