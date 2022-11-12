<<<<<<< HEAD
﻿
using System.Threading.Tasks;

namespace Nop.Web.Framework.Themes
{
    /// <summary>
    /// Represents a theme context
    /// </summary>
    public partial interface IThemeContext
    {
        /// <summary>
        /// Get current theme system name
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task<string> GetWorkingThemeNameAsync();

        /// <summary>
        /// Set current theme system name
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task SetWorkingThemeNameAsync(string workingThemeName);
    }
}
=======
﻿
using System.Threading.Tasks;

namespace Nop.Web.Framework.Themes
{
    /// <summary>
    /// Represents a theme context
    /// </summary>
    public partial interface IThemeContext
    {
        /// <summary>
        /// Get current theme system name
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task<string> GetWorkingThemeNameAsync();

        /// <summary>
        /// Set current theme system name
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task SetWorkingThemeNameAsync(string workingThemeName);
    }
}
>>>>>>> 174426a8e1a9c69225a65c26a93d9aa871080855
