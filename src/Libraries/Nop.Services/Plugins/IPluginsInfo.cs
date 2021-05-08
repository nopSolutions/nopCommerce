using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nop.Services.Plugins
{
    /// <summary>
    /// Represents an information about plugins
    /// </summary>
    public interface IPluginsInfo
    {
        /// <summary>
        /// Save plugins info to the file
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task SaveAsync();

        /// <summary>
        /// Save plugins info to the file
        /// </summary>
        void Save();

        /// <summary>
        /// Get plugins info
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the rue if data are loaded, otherwise False
        /// </returns>
        Task<bool> LoadPluginInfoAsync();

        /// <summary>
        /// Create copy from another instance of IPluginsInfo interface
        /// </summary>
        /// <param name="pluginsInfo">Plugins info</param>
        void CopyFrom(IPluginsInfo pluginsInfo);
        
        /// <summary>
        /// Gets or sets the list of all installed plugin
        /// </summary>
        IList<PluginDescriptorBaseInfo> InstalledPlugins { get; set; }

        /// <summary>
        /// Gets or sets the list of plugin names which will be uninstalled
        /// </summary>
        IList<string> PluginNamesToUninstall { get; set; }

        /// <summary>
        /// Gets or sets the list of plugin names which will be deleted
        /// </summary>
        IList<string> PluginNamesToDelete { get; set; }

        /// <summary>
        /// Gets or sets the list of plugin names which will be installed
        /// </summary>
        IList<(string SystemName, Guid? CustomerGuid)> PluginNamesToInstall { get; set; }

        /// <summary>
        /// Gets or sets the list of assembly loaded collisions
        /// </summary>
        IList<PluginLoadedAssemblyInfo> AssemblyLoadedCollision { get; set; }

        /// <summary>
        /// Gets or sets a collection of plugin descriptors of all deployed plugins
        /// </summary>
        IList<PluginDescriptor> PluginDescriptors { get; set; }

        /// <summary>
        /// Gets or sets the list of plugin names which are not compatible with the current version
        /// </summary>
        IList<string> IncompatiblePlugins { get; set; }
    }
}