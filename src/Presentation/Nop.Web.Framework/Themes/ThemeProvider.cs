using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Nop.Core;

namespace Nop.Web.Framework.Themes
{
    /// <summary>
    /// Represents a default theme provider implementation
    /// </summary>
    public partial class ThemeProvider : IThemeProvider
    {
        #region Constants

        private const string ThemesPath = "~/Themes/";
        private const string ThemeConfigurationFileName = "theme.config";

        #endregion

        #region Fields
        
        private IList<ThemeConfiguration> _themeConfigurations;

        #endregion

        #region Utilities

        private void LoadConfigurations()
        {
            _themeConfigurations = new List<ThemeConfiguration>();
            foreach (string themeName in Directory.GetDirectories(CommonHelper.MapPath(ThemesPath)))
            {
                var configuration = CreateThemeConfiguration(themeName);
                if (configuration != null)
                {
                    _themeConfigurations.Add(configuration);
                }
            }
        }

        private ThemeConfiguration CreateThemeConfiguration(string themePath)
        {
            var themeDirectory = new DirectoryInfo(themePath);
            var themeConfigFile = new FileInfo(Path.Combine(themeDirectory.FullName, ThemeConfigurationFileName));

            if (themeConfigFile.Exists)
            {
                var doc = new XmlDocument();
                doc.Load(themeConfigFile.FullName);
                return new ThemeConfiguration(themeDirectory.Name, doc);
            }

            return null;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Get all theme configurations
        /// </summary>
        /// <returns>List of the theme configuration</returns>
        public IList<ThemeConfiguration> GetThemeConfigurations()
        {
            if (_themeConfigurations == null)
            {
                //load all theme configurations
                LoadConfigurations();
            }

            return _themeConfigurations;
        }

        /// <summary>
        /// Get theme configuration by theme system name
        /// </summary>
        /// <param name="systemName">Theme system name</param>
        /// <returns>Theme configuration</returns>
        public ThemeConfiguration GetThemeConfigurationBySystemName(string systemName)
        {
            if (string.IsNullOrEmpty(systemName))
                return null;

            return GetThemeConfigurations().SingleOrDefault(configuration => configuration.SystemName.Equals(systemName, StringComparison.InvariantCultureIgnoreCase));
        }

        /// <summary>
        /// Check whether theme configuration with specified system name exists
        /// </summary>
        /// <param name="systemName">Theme system name</param>
        /// <returns>True if theme configuration exists; otherwise false</returns>
        public bool ThemeConfigurationExists(string systemName)
        {
            if (string.IsNullOrEmpty(systemName))
                return false;

            return GetThemeConfigurations().Any(configuration => configuration.SystemName.Equals(systemName, StringComparison.InvariantCultureIgnoreCase));
        }

        #endregion
    }
}