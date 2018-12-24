using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Nop.Core.Domain.Customers;
using Nop.Core.Infrastructure;

namespace Nop.Core.Plugins
{
    /// <summary>
    /// Represents an information about plugins
    /// </summary>
    public class PluginsInfo
    {
        #region Fields

        private INopFileProvider _fileProvider;

        #endregion

        #region Utilities
        
        /// <summary>
        /// Get system names of installed plugins from obsolete file
        /// </summary>
        /// <param name="filePath">Path to the file</param>
        /// <param name="fileProvider">File provider</param>
        /// <returns>List of plugin system names</returns>
        private static IList<string> GetObsoleteInstalledPluginNames(string filePath, INopFileProvider fileProvider)
        {
            //check whether file exists
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
        /// Checks whether to perform a plugin
        /// </summary>
        /// <param name="pluginSystemName">Plugin system name</param>
        /// <returns>True if need to perform a plugin, False - otherwise</returns>
        public bool NeedToPerform(string pluginSystemName)
        {
            var needToPerform = InstalledPluginNames.Any(systemName =>
                systemName.Equals(pluginSystemName, StringComparison.CurrentCultureIgnoreCase));

            needToPerform = needToPerform || IsPluginNeedToInstall(pluginSystemName);

            needToPerform = needToPerform && !PluginNamesToDelete.Any(systemName =>
                                systemName.Equals(pluginSystemName, StringComparison.CurrentCultureIgnoreCase));

            return needToPerform;
        }

        /// <summary>
        /// Save plugins info to the file
        /// </summary>
        public void Save()
        {
            //save the file
            var text = JsonConvert.SerializeObject(this, Formatting.Indented);
            _fileProvider.WriteAllText(_fileProvider.MapPath(NopPluginDefaults.PluginsInfoFilePath), text,
                Encoding.UTF8);
        }

        /// <summary>
        /// Get plugins info
        /// </summary>
        /// <returns>Plugins info</returns>
        public static PluginsInfo LoadPluginInfo(INopFileProvider fileProvider)
        {
            var filePath = fileProvider.MapPath(NopPluginDefaults.PluginsInfoFilePath);

            //check whether file exists
            PluginsInfo info;
            if (!fileProvider.FileExists(filePath))
            {
                info = new PluginsInfo
                {
                    InstalledPluginNames =
                        GetObsoleteInstalledPluginNames(
                            fileProvider.MapPath(NopPluginDefaults.InstalledPluginsFilePath), fileProvider),
                    _fileProvider = fileProvider
                };

                info.Save();
                return info;
            }

            var text = fileProvider.ReadAllText(filePath, Encoding.UTF8);

            //get plugin info from the JSON file
            info = string.IsNullOrEmpty(text) ? new PluginsInfo() : JsonConvert.DeserializeObject<PluginsInfo>(text);
            info._fileProvider = fileProvider;

            return info;
        }

        /// <summary>
        /// Add plugin to the installations list
        /// </summary>
        /// <param name="systemName">Plugin system name</param>
        /// <param name="customer">Customer</param>
        public void AddToInstall(string systemName, Customer customer = null)
        {
            if (IsPluginNeedToInstall(systemName))
                return;

            PluginNamesToInstall.Add(new PluginToInstall
            {
                SystemName = systemName,
                CustomerGuid = customer?.CustomerGuid
            });

            Save();
        }

        /// <summary>
        /// Remove plugin from "to the installations" list
        /// </summary>
        /// <param name="systemName">Plugin system name</param>
        public PluginToInstall RemoveFromToInstallList(string systemName)
        {
            var info = PluginNamesToInstall.FirstOrDefault(pluginName =>
                pluginName.SystemName.Equals(systemName, StringComparison.CurrentCultureIgnoreCase));

            PluginNamesToInstall.Remove(info);

            Save();

            return info;
        }

        /// <summary>
        /// Add plugin to the uninstallations list
        /// </summary>
        /// <param name="systemName">Plugin system name</param>
        public void AddToUnInstall(string systemName)
        {
            if (PluginNamesToUninstall.Any(p =>
                p.Equals(systemName, StringComparison.CurrentCultureIgnoreCase)))
                return;

            PluginNamesToUninstall.Add(systemName);

            Save();
        }

        /// <summary>
        /// Add plugin to the deletions list
        /// </summary>
        /// <param name="systemName">Plugin system name</param>
        public void AddToDelete(string systemName)
        {
            if (PluginNamesToDelete.Any(p =>
                p.Equals(systemName, StringComparison.CurrentCultureIgnoreCase)))
                return;

            PluginNamesToDelete.Add(systemName);

            Save();
        }
        
        /// <summary>
        /// Reset changes
        /// </summary>
        public void ResetChanges()
        {
            PluginNamesToDelete.Clear();
            PluginNamesToInstall.Clear();
            PluginNamesToUninstall.Clear();

            foreach (var pluginDescriptor in PluginManager.ReferencedPlugins.Where(p => !p.ShowInPluginsList))
            {
                pluginDescriptor.ShowInPluginsList = true;
            }

            Save();
        }

        /// <summary>
        /// Check is the plugin need to be installed
        /// </summary>
        /// <param name="systemName">Plugin system name</param>
        /// <returns>True if plugin need to be installed, else false</returns>
        public bool IsPluginNeedToInstall(string systemName)
        {
            return PluginNamesToInstall.Any(pluginName => pluginName.SystemName.Equals(systemName, StringComparison.CurrentCultureIgnoreCase));
        }
        
        #endregion

        #region Properties

        /// <summary>
        /// Installed plugin names
        /// </summary>
        public IList<string> InstalledPluginNames { get; set; } = new List<string>();

        /// <summary>
        /// List of plugin names which will be installed
        /// </summary>
        public IList<PluginToInstall> PluginNamesToInstall { get; set; } = new List<PluginToInstall>();

        /// <summary>
        /// List of plugin names which will be uninstalled
        /// </summary>
        public IList<string> PluginNamesToUninstall { get; set; } = new List<string>();

        /// <summary>
        /// List of plugin names which will be deleted
        /// </summary>
        public IList<string> PluginNamesToDelete { get; set; } = new List<string>();

        #endregion

        /// <summary>
        /// Represents a plugin to install info
        /// </summary>
        public class PluginToInstall
        {
            /// <summary>
            /// Plugin's system name
            /// </summary>
            public string SystemName { get; set; }

            /// <summary>
            /// Customer's GUID, who install this plugin
            /// </summary>
            public Guid? CustomerGuid { get; set; }
        }
    }
}