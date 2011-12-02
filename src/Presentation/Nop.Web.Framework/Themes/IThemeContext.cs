namespace Nop.Web.Framework.Themes
{
    /// <summary>
    /// Work context
    /// </summary>
    public interface IThemeContext
    {
        /// <summary>
        /// Get or set current theme for desktops (e.g. darkOrange)
        /// </summary>
        string WorkingDesktopTheme { get; set; }
    }
}
