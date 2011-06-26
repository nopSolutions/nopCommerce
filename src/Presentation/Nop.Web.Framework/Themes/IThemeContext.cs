namespace Nop.Web.Framework.Themes
{
    /// <summary>
    /// Work context
    /// </summary>
    public interface IThemeContext
    {
        /// <summary>
        /// Get or set current graphical theme (e.g. darkOrange)
        /// </summary>
        string WorkingTheme { get; set; }
    }
}
