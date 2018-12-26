using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Nop.Core.Infrastructure;

namespace Nop.Core.Plugins
{
    /// <summary>
    /// Represents an information about plugins
    /// </summary>
    public partial class PluginsInfo
    {
        #region Utilities

        /// <summary>
        /// Get system names of installed plugins from obsolete file
        /// </summary>
        /// <param name="fileProvider">File provider</param>
        /// <returns>List of plugin system names</returns>
        private static IList<string> GetObsoleteInstalledPluginNames(INopFileProvider fileProvider)
        {
            //check whether file exists
            var filePath = fileProvider.MapPath(NopPluginDefaults.InstalledPluginsFilePath);
            if (!fileProvider.FileExists(filePath))
            {
                //if not, try to parse the file that was used in previous nopCommerce versions
                filePath = fileProvider.MapPath(NopPluginDefaults.ObsoleteInstalledPluginsFilePath);
                if (!fileProvider.FileExists(filePath))
                    return new List<string>();

                //get plugin system names from the old txt file
                var pluginSystemNames = new List<string>();
                using (var reader = new StringReader(fileProvider.ReadAllText(filePath, Encoding.UTF8)))
                {
                    string pluginName;
                    while ((pluginName = reader.ReadLine()) != null)
                    {
                        if (!string.IsNullOrWhiteSpace(pluginName))
                            pluginSystemNames.Add(pluginName.Trim());
                    }
                }

                //and delete the old one
                fileProvider.DeleteFile(filePath);

                return pluginSystemNames;
            }

            var text = fileProvider.ReadAllText(filePath, Encoding.UTF8);
            if (string.IsNullOrEmpty(text))
                return new List<string>();

            //delete the old file
            fileProvider.DeleteFile(filePath);

            //get plugin system names from the JSON file
            return JsonConvert.DeserializeObject<IList<string>>(text);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Save plugins info to the file
        /// </summary>
        /// <param name="fileProvider">File provider</param>
        public void Save(INopFileProvider fileProvider)
        {
            //save the file
            var filePath = fileProvider.MapPath(NopPluginDefaults.PluginsInfoFilePath);
            var text = JsonConvert.SerializeObject(this, Formatting.Indented);
            fileProvider.WriteAllText(filePath, text, Encoding.UTF8);
        }

        /// <summary>
        /// Get plugins info
        /// </summary>
        /// <returns>Plugins info</returns>
        public static PluginsInfo LoadPluginInfo(INopFileProvider fileProvider)
        {
            //check whether plugins info file exists
            var filePath = fileProvider.MapPath(NopPluginDefaults.PluginsInfoFilePath);
            if (!fileProvider.FileExists(filePath))
            {
                //file doesn't exist, so try to get only installed plugin names from the obsolete file
                var pluginsInfo = new PluginsInfo
                {
                    InstalledPluginNames = GetObsoleteInstalledPluginNames(fileProvider)
                };

                //and save info into a new file
                pluginsInfo.Save(fileProvider);

                return pluginsInfo;
            }

            //try to get plugin info from the JSON file
            var text = fileProvider.ReadAllText(filePath, Encoding.UTF8);
            if (string.IsNullOrEmpty(text))
                return new PluginsInfo();

            return JsonConvert.DeserializeObject<PluginsInfo>(text);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the list of all installed plugin names
        /// </summary>
        public IList<string> InstalledPluginNames { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets the list of plugin names which will be uninstalled
        /// </summary>
        public IList<string> PluginNamesToUninstall { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets the list of plugin names which will be deleted
        /// </summary>
        public IList<string> PluginNamesToDelete { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets the list of plugin names which will be installed
        /// </summary>
        public IList<(string SystemName, Guid? CustomerGuid)> PluginNamesToInstall { get; set; } = new List<(string SystemName, Guid? CustomerGuid)>();

        /// <summary>
        /// Gets or sets the list of plugin names which are not compatible with the current version
        /// </summary>
        [JsonIgnore]
        public IList<string> IncompatiblePlugins { get; set; }

        /// <summary>
        /// Gets or sets the list of assembly loaded collisions
        /// </summary>
        [JsonIgnore]
        public IList<PluginLoadedAssemblyInfo> AssemblyLoadedCollision { get; set; }

        /// <summary>
        /// Gets or sets a collection of plugin descriptors of all deployed plugins
        /// </summary>
        [JsonIgnore]
        public IEnumerable<PluginDescriptor> PluginDescriptors { get; set; }

        #endregion
    }
}