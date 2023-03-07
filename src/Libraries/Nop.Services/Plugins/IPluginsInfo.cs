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
        /// Get plugins info
        /// </summary>
        /// <returns>
        /// The true if data are loaded, otherwise False
        /// </returns>
        void LoadPluginInfo();

        /// <summary>
        /// Save plugins info to the file
        /// </summary>
        void Save();

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
        IList<(PluginDescriptor pluginDescriptor, bool needToDeploy)> PluginDescriptors { get; set; }

        /// <summary>
        /// Gets or sets the list of plugin which are not compatible with the current version
        /// </summary>
        /// <remarks>
        /// Key - the system name of plugin.
        /// Value - the incompatibility type.
        /// </remarks>
        IDictionary<string, PluginIncompatibleType> IncompatiblePlugins { get; set; }
    }
}