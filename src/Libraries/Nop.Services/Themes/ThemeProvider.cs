using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Nop.Core;

namespace Nop.Services.Themes
{
    /// <summary>
    /// Represents a default theme provider implementation
    /// </summary>
    public partial class ThemeProvider : IThemeProvider
    {
        #region Fields

        private IList<ThemeDescriptor> _themeDescriptors;

        #endregion

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
            if (_themeDescriptors == null)
            {
                //load all theme descriptors
                var themeFolder = new DirectoryInfo(CommonHelper.MapPath(ThemesPath));
                _themeDescriptors = new List<ThemeDescriptor>();
                foreach (var descriptionFile in themeFolder.GetFiles(ThemeDescriptionFileName, SearchOption.AllDirectories))
                {
                    var text = File.ReadAllText(descriptionFile.FullName);
                    if (string.IsNullOrEmpty(text))
                        continue;

                    //get theme descriptor
                    var themeDescriptor = GetThemeDescriptorFromText(text);

                    //some validation
                    if (string.IsNullOrEmpty(themeDescriptor?.SystemName))
                        throw new Exception($"A theme descriptor '{descriptionFile.FullName}' has no system name");

                    _themeDescriptors.Add(themeDescriptor);
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

        #region Properties

        /// <summary>
        /// Gets the path to themes folder
        /// </summary>
        public string ThemesPath => "~/Themes";

        /// <summary>
        /// Gets the name of the theme description file
        /// </summary>
        public string ThemeDescriptionFileName => "theme.json";

        #endregion
    }
}