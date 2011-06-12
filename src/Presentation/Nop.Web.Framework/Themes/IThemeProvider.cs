using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nop.Web.Framework.Themes
{
    public interface IThemeProvider
    {
        ThemeConfiguration GetThemeConfiguration(string themeName);

        IList<ThemeConfiguration> GetThemeConfigurations();

        bool ThemeConfigurationExists(string themeName);
    }
}
