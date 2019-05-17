using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Nop.Core.Infrastructure;
using Nop.Services.Plugins;

namespace Nop.Services.Themes
{
    /// <summary>
    /// Represents a default theme provider implementation
    /// </summary>
    public partial class ThemeProvider : IThemeProvider
    {
        #region Fields

        private readonly INopFileProvider _fileProvider;

        private IList<ThemeDescriptor> _themeDescriptors;

        #endregion

        public ThemeProvider(INopFileProvider fileProvider)
        {
            _fileProvider = fileProvider;
        }

        #region Methods
        /// <summary>
        /// Get theme descriptor from the description text
        /// </summary>
        /// <param name="text">Description text</param>
        /// <returns>Theme descriptor</returns>
        public ThemeDescriptor GetThemeDescriptorFromText(string text)
        {
            //get theme description from the JSON file
            var themeDescriptor = JsonConvert.DeserializeObject<ThemeDescriptor>(text);

            //some validation
            if (_themeDescriptors?.Any(descriptor => descriptor.SystemName.Equals(themeDescriptor?.SystemName, StringComparison.InvariantCultureIgnoreCase)) ?? false)
                throw new Exception($"A theme with '{themeDescriptor.SystemName}' system name is already defined");

            return themeDescriptor;
        }

        /// <summary>
        /// Get all themes
        /// </summary>
        /// <returns>List of the theme descriptor</returns>
        public IList<ThemeDescriptor> GetThemes()
        {
            if (_themeDescriptors != null)
                return _themeDescriptors;

            //load all theme descriptors
            _themeDescriptors = new List<ThemeDescriptor>();

            var themeDirectoryPath = _fileProvider.MapPath(NopPluginDefaults.ThemesPath);
            foreach (var descriptionFile in _fileProvider.GetFiles(themeDirectoryPath, NopPluginDefaults.ThemeDescriptionFileName, false))
            {
                var text = _fileProvider.ReadAllText(descriptionFile, Encoding.UTF8);
                if (string.IsNullOrEmpty(text))
                    continue;

                //get theme descriptor
                var themeDescriptor = GetThemeDescriptorFromText(text);

                //some validation
                if (string.IsNullOrEmpty(themeDescriptor?.SystemName))
                    throw new Exception($"A theme descriptor '{descriptionFile}' has no system name");

                _themeDescriptors.Add(themeDescriptor);
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