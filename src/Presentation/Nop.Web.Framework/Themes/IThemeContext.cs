
using System.Threading.Tasks;

namespace Nop.Web.Framework.Themes
{
    /// <summary>
    /// Represents a theme context
    /// </summary>
    public interface IThemeContext
    {
        /// <summary>
        /// Get current theme system name
        /// </summary>
        Task<string> GetWorkingThemeName();

        /// <summary>
        /// Set current theme system name
        /// </summary>
        Task SetWorkingThemeName(string workingThemeName);
    }
}
