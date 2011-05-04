
using System;
using System.Linq;
using System.Web;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Tax;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Localization;

namespace Nop.Web.Framework.Themes
{
    /// <summary>
    /// Theme context
    /// </summary>
    public partial class ThemeContext : IThemeContext
    {
        private string _workingTheme = "";

        /// <summary>
        /// Get or set current theme (e.g. darkOrange)
        /// </summary>
        public string WorkingTheme
        {
            get
            {
                return _workingTheme;
            }
            set
            {
                _workingTheme = value;
            }
        }
    }
}
