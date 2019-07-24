
namespace Nop.Web.Framework.Themes
{
    /// <summary>
    /// Represents a theme context
    /// </summary>
    public interface IThemeContext
    {
        /// <summary>
        /// Get or set current theme system name
        /// </summary>
        string WorkingThemeName { get; set; }
    }
}
