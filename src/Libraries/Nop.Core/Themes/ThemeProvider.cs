using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace Nop.Core.Themes
{
    /// <summary>
    /// Represents a default theme provider implementation
    /// </summary>
    public partial class ThemeProvider : IThemeProvider
    {
        #region Constants

        private const string ThemesPath = "~/Themes/";
        private const string ThemeDescriptionFileName = "theme.json";

        #endregion

        #region Fields

        private IList<ThemeDescriptor> _themeDescriptors;

        #endregion

        #region Utilities

        /// <summary>
        /// Get theme descriptor from the file
        /// </summary>
        /// <param name="filePath">Path to the description file</param>
        /// <returns>Theme descriptor</returns>
        protected virtual ThemeDescriptor GeThemeDescriptor(string filePath)
        {
            var text = File.ReadAllText(filePath);
            if (string.IsNullOrEmpty(text))
                return new ThemeDescriptor();

            //get theme description from the JSON file
            var themeDescriptor = JsonConvert.DeserializeObject<ThemeDescriptor>(text);

            //some validation
            if (string.IsNullOrEmpty(themeDescriptor?.SystemName))
                throw new Exception($"A theme descriptor '{filePath}' has no system name");

            if (_themeDescriptors?.Any(descriptor => descriptor.SystemName.Equals(themeDescriptor.SystemName, StringComparison.InvariantCultureIgnoreCase)) ?? false)
                throw new Exception($"A theme with '{themeDescriptor.SystemName}' system name is already defined");

            return themeDescriptor;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Get all themes
        /// </summary>
        /// <returns>List of the theme descriptor</returns>
        public IList<ThemeDescriptor> GetThemes()
        {
            if (_themeDescriptors == null)
            {
                //load all theme descriptors
                var themeFolder = new DirectoryInfo(CommonHelper.MapPath(ThemesPath));
                _themeDescriptors = new List<ThemeDescriptor>();
                foreach (var descriptionFile in themeFolder.GetFiles(ThemeDescriptionFileName, SearchOption.AllDirectories))
                {
                    _themeDescriptors.Add(GeThemeDescriptor(descriptionFile.FullName));
                }
            }

            return _themeDescriptors;
        }

        /// <summary>
        /// Get a theme by the system name
        /// </summary>
        /// <param name="systemName">Theme system name</param>
        /// <returns>Theme descriptor</returns>
        public ThemeDescriptor GetThemeBySystemName(string systemName)
        {
            if (string.IsNullOrEmpty(systemName))
                return null;

            return GetThemes().SingleOrDefault(descriptor => descriptor.SystemName.Equals(systemName, StringComparison.InvariantCultureIgnoreCase));
        }

        /// <summary>
        /// Check whether the theme with specified system name exists
        /// </summary>
        /// <param name="systemName">Theme system name</param>
        /// <returns>True if the theme exists; otherwise false</returns>
        public bool ThemeExists(string systemName)
        {
            if (string.IsNullOrEmpty(systemName))
                return false;

            return GetThemes().Any(descriptor => descriptor.SystemName.Equals(systemName, StringComparison.InvariantCultureIgnoreCase));
        }

        #endregion
    }
}