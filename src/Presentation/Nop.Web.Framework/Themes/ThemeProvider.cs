using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
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
        private const string ThemeConfigurationFileName = "theme.json";

        #endregion

        #region Fields

        private IList<ThemeConfiguration> _themeConfigurations;

        #endregion

        #region Utilities

        /// <summary>
        /// Get theme configuration from the file
        /// </summary>
        /// <param name="filePath">Path to the configuration file</param>
        /// <returns>Theme configuration</returns>
        protected virtual ThemeConfiguration GeThemeConfiguration(string filePath)
        {
            var text = File.ReadAllText(filePath);
            if (string.IsNullOrEmpty(text))
                return new ThemeConfiguration();

            //get theme configuration from the JSON file
            var themeConfiguration = JsonConvert.DeserializeObject<ThemeConfiguration>(text);

            //some validation
            if (string.IsNullOrEmpty(themeConfiguration?.SystemName))
                throw new Exception($"A theme configuration '{filePath}' has no system name");

            if (_themeConfigurations?.Any(configuration => configuration.SystemName.Equals(themeConfiguration.SystemName, StringComparison.InvariantCultureIgnoreCase)) ?? false)
                throw new Exception($"A theme with '{themeConfiguration.SystemName}' system name is already defined");

            return themeConfiguration;
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
                var themeFolder = new DirectoryInfo(CommonHelper.MapPath(ThemesPath));
                _themeConfigurations = new List<ThemeConfiguration>();
                foreach (var configurationFile in themeFolder.GetFiles(ThemeConfigurationFileName, SearchOption.AllDirectories))
                {
                    _themeConfigurations.Add(GeThemeConfiguration(configurationFile.FullName));
                }
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