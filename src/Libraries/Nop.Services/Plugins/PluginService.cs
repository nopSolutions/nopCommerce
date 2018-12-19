using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Plugins;
using Nop.Services.Customers;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Services.Logging;

namespace Nop.Services.Plugins
{
    /// <summary>
    /// Plugin finder
    /// </summary>
    public class PluginService : IPluginService
    {
        #region Fields

        private readonly ICustomerActivityService _customerActivityService;
        private readonly ICustomerService _customerService;
        private readonly IEventPublisher _eventPublisher;
        private readonly ILocalizationService _localizationService;
        private readonly ILogger _logger;
        private readonly IWorkContext _workContext;

        private IList<PluginDescriptor> _plugins;
        private bool _arePluginsLoaded;
        private List<PluginsInfo.PluginToInstall> _lastInstalledPluginSystemNames;

        #endregion

        #region Ctor

        public PluginService(ICustomerActivityService customerActivityService,
            ICustomerService customerService,
            IEventPublisher eventPublisher,
            ILocalizationService localizationService,
            ILogger logger,
            IWorkContext workContext)
        {
            this._customerActivityService = customerActivityService;
            this._customerService = customerService;
            this._eventPublisher = eventPublisher;
            this._localizationService = localizationService;
            this._logger = logger;
            this._workContext = workContext;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Ensure plugins are loaded
        /// </summary>
        protected virtual void EnsurePluginsAreLoaded()
        {
            if (_arePluginsLoaded) 
                return;

            var foundPlugins = PluginManager.ReferencedPlugins.ToList();
            foundPlugins.Sort();
            _plugins = foundPlugins.ToList();

            _arePluginsLoaded = true;
        }

        /// <summary>
        /// Check whether the plugin is available in a certain store
        /// </summary>
        /// <param name="pluginDescriptor">Plugin descriptor to check</param>
        /// <param name="loadMode">Load plugins mode</param>
        /// <returns>true - available; false - no</returns>
        protected virtual bool CheckLoadMode(PluginDescriptor pluginDescriptor, LoadPluginsMode loadMode)
        {
            if (pluginDescriptor == null)
                throw new ArgumentNullException(nameof(pluginDescriptor));

            switch (loadMode)
            {
                case LoadPluginsMode.All:
                    //no filtering
                    return true;
                case LoadPluginsMode.InstalledOnly:
                    return pluginDescriptor.Installed;
                case LoadPluginsMode.NotInstalledOnly:
                    return !pluginDescriptor.Installed;
                default:
                    throw new Exception("Not supported LoadPluginsMode");
            }
        }

        /// <summary>
        /// Check whether the plugin is in a certain group
        /// </summary>
        /// <param name="pluginDescriptor">Plugin descriptor to check</param>
        /// <param name="group">Group</param>
        /// <returns>true - available; false - no</returns>
        protected virtual bool CheckGroup(PluginDescriptor pluginDescriptor, string group)
        {
            if (pluginDescriptor == null)
                throw new ArgumentNullException(nameof(pluginDescriptor));

            if (string.IsNullOrEmpty(group))
                return true;

            return group.Equals(pluginDescriptor.Group, StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Uninstall plugins if need
        /// </summary>
        /// <returns>List of uninstalled plugin system names</returns>
        protected virtual List<string> UninstallPluginsIfNeed()
        {
            var uninstalledPluginSystemNames = new List<string>();

            //uninstall plugins
            foreach (var pluginDescriptor in PluginManager.ReferencedPlugins)
            {
                if (!pluginDescriptor.Installed || !PluginManager.PluginsInfo.PluginNamesToUninstall.Any(systemName => systemName.Equals(pluginDescriptor.SystemName, StringComparison.CurrentCultureIgnoreCase)))
                    continue;

                try
                {
                    pluginDescriptor.Instance().Uninstall();

                    //remove plugin system name from the list if exists
                    var alreadyMarkedAsInstalled = PluginManager.PluginsInfo.InstalledPluginNames.Any(pluginName => pluginName.Equals(pluginDescriptor.SystemName, StringComparison.InvariantCultureIgnoreCase));
                    if (alreadyMarkedAsInstalled)
                        PluginManager.PluginsInfo.InstalledPluginNames.Remove(pluginDescriptor.SystemName);

                    PluginManager.PluginsInfo.PluginNamesToUninstall.Remove(pluginDescriptor.SystemName);

                    uninstalledPluginSystemNames.Add(pluginDescriptor.SystemName);

                    pluginDescriptor.Installed = false;
                    pluginDescriptor.ShowInPluginsList = true;
                }
                catch (Exception ex)
                {
                    pluginDescriptor.Error = ex;
                }
            }

            PluginManager.PluginsInfo.Save();

            return uninstalledPluginSystemNames;
        }

        /// <summary>
        /// Delete plugins if need
        /// </summary>
        /// <returns>List of deleted plugin system names</returns>
        protected virtual List<string> DeletePluginsIfNeed()
        {
            var deletedPluginSystemNames = new List<string>();

            //delete plugins
            foreach (var dfd in PluginManager.PluginDescriptors)
            {
                var pluginDescriptor = PluginManager.ReferencedPlugins.FirstOrDefault(p => p.SystemName.Equals(dfd.SystemName, StringComparison.CurrentCultureIgnoreCase));

                if (pluginDescriptor == null)
                    continue;

                if (pluginDescriptor.Installed || !PluginManager.PluginsInfo.PluginNamesToDelete.Any(systemName => systemName.Equals(pluginDescriptor.SystemName, StringComparison.CurrentCultureIgnoreCase)))
                    continue;

                try
                {
                    if (pluginDescriptor.DeletePlugin())
                    {
                        PluginManager.PluginsInfo.PluginNamesToDelete.Remove(pluginDescriptor.SystemName);
                        deletedPluginSystemNames.Add(pluginDescriptor.SystemName);
                    }
                }
                catch (Exception ex)
                {
                    pluginDescriptor.Error = ex;
                }
            }

            PluginManager.PluginsInfo.Save();

            return deletedPluginSystemNames;
        }

        /// <summary>
        /// Install plugins if need
        /// </summary>
        /// <returns></returns>
        protected virtual void InstallPluginsIfNeed()
        {
            _lastInstalledPluginSystemNames = new List<PluginsInfo.PluginToInstall>();

            foreach (var dfd in PluginManager.PluginDescriptors)
            {
                var pluginDescriptor = dfd;

                if (pluginDescriptor.Installed || !PluginManager.PluginsInfo.IsPluginNeedToInstall(pluginDescriptor.SystemName))
                    continue;

                var loadedDescriptor = PluginManager.ReferencedPlugins.FirstOrDefault(p =>
                    p.SystemName.Equals(pluginDescriptor.SystemName, StringComparison.CurrentCultureIgnoreCase));

                if (loadedDescriptor == null)
                    continue;

                try
                {
                    loadedDescriptor.Instance().Install();

                    //add plugin system name to the list if doesn't already exist
                    var alreadyMarkedAsInstalled = PluginManager.PluginsInfo.InstalledPluginNames.Any(pluginName => pluginName.Equals(pluginDescriptor.SystemName, StringComparison.InvariantCultureIgnoreCase));
                    if (!alreadyMarkedAsInstalled)
                    {
                        PluginManager.PluginsInfo.InstalledPluginNames.Add(pluginDescriptor.SystemName);
                    }

                    _lastInstalledPluginSystemNames.Add(PluginManager.PluginsInfo.RemoveFromToInstallList(loadedDescriptor.SystemName));

                    loadedDescriptor.Installed = true;
                }
                catch (Exception ex)
                {
                    loadedDescriptor.Error = ex;
                }
            }

            PluginManager.PluginsInfo.Save();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Check whether the plugin is available in a certain store
        /// </summary>
        /// <param name="pluginDescriptor">Plugin descriptor to check</param>
        /// <param name="storeId">Store identifier to check</param>
        /// <returns>true - available; false - no</returns>
        public virtual bool AuthenticateStore(PluginDescriptor pluginDescriptor, int storeId)
        {
            if (pluginDescriptor == null)
                throw new ArgumentNullException(nameof(pluginDescriptor));

            //no validation required
            if (storeId == 0)
                return true;

            if (!pluginDescriptor.LimitedToStores.Any())
                return true;

            return pluginDescriptor.LimitedToStores.Contains(storeId);
        }

        /// <summary>
        /// Check that plugin is available for the specified customer
        /// </summary>
        /// <param name="pluginDescriptor">Plugin descriptor to check</param>
        /// <param name="customer">Customer</param>
        /// <returns>True if authorized; otherwise, false</returns>
        public virtual bool AuthorizedForUser(PluginDescriptor pluginDescriptor, Customer customer)
        {
            if (pluginDescriptor == null)
                throw new ArgumentNullException(nameof(pluginDescriptor));

            if (customer == null || !pluginDescriptor.LimitedToCustomerRoles.Any())
                return true;

            var customerRoleIds = customer.CustomerRoles.Where(role => role.Active).Select(role => role.Id);

            return pluginDescriptor.LimitedToCustomerRoles.Intersect(customerRoleIds).Any();
        }

        /// <summary>
        /// Gets plugin groups
        /// </summary>
        /// <returns>Plugins groups</returns>
        public virtual IEnumerable<string> GetPluginGroups()
        {
            return GetPluginDescriptors(LoadPluginsMode.All).Select(x => x.Group).Distinct().OrderBy(x => x);
        }

        /// <summary>
        /// Gets plugins
        /// </summary>
        /// <typeparam name="T">The type of plugins to get.</typeparam>
        /// <param name="loadMode">Load plugins mode</param>
        /// <param name="customer">Load records allowed only to a specified customer; pass null to ignore ACL permissions</param>
        /// <param name="storeId">Load records allowed only in a specified store; pass 0 to load all records</param>
        /// <param name="group">Filter by plugin group; pass null to load all records</param>
        /// <returns>Plugins</returns>
        public virtual IEnumerable<T> GetPlugins<T>(LoadPluginsMode loadMode = LoadPluginsMode.InstalledOnly,
            Customer customer = null, int storeId = 0, string group = null) where T : class, IPlugin
        {
            return GetPluginDescriptors<T>(loadMode, customer, storeId, group).Select(p => p.Instance<T>());
        }

        /// <summary>
        /// Get plugin descriptors
        /// </summary>
        /// <param name="loadMode">Load plugins mode</param>
        /// <param name="customer">Load records allowed only to a specified customer; pass null to ignore ACL permissions</param>
        /// <param name="storeId">Load records allowed only in a specified store; pass 0 to load all records</param>
        /// <param name="group">Filter by plugin group; pass null to load all records</param>
        /// <returns>Plugin descriptors</returns>
        public virtual IEnumerable<PluginDescriptor> GetPluginDescriptors(LoadPluginsMode loadMode = LoadPluginsMode.InstalledOnly,
            Customer customer = null, int storeId = 0, string group = null)
        {
            //ensure plugins are loaded
            EnsurePluginsAreLoaded();

            return _plugins.Where(p => CheckLoadMode(p, loadMode) && AuthorizedForUser(p, customer) && AuthenticateStore(p, storeId) && CheckGroup(p, group));
        }

        /// <summary>
        /// Get plugin descriptors
        /// </summary>
        /// <typeparam name="T">The type of plugin to get.</typeparam>
        /// <param name="loadMode">Load plugins mode</param>
        /// <param name="customer">Load records allowed only to a specified customer; pass null to ignore ACL permissions</param>
        /// <param name="storeId">Load records allowed only in a specified store; pass 0 to load all records</param>
        /// <param name="group">Filter by plugin group; pass null to load all records</param>
        /// <returns>Plugin descriptors</returns>
        public virtual IEnumerable<PluginDescriptor> GetPluginDescriptors<T>(LoadPluginsMode loadMode = LoadPluginsMode.InstalledOnly,
            Customer customer = null, int storeId = 0, string group = null) 
            where T : class, IPlugin
        {
            return GetPluginDescriptors(loadMode, customer, storeId, group)
                .Where(p => typeof(T).IsAssignableFrom(p.PluginType));
        }

        /// <summary>
        /// Get a plugin descriptor by its system name
        /// </summary>
        /// <param name="systemName">Plugin system name</param>
        /// <param name="loadMode">Load plugins mode</param>
        /// <returns>>Plugin descriptor</returns>
        public virtual PluginDescriptor GetPluginDescriptorBySystemName(string systemName, LoadPluginsMode loadMode = LoadPluginsMode.InstalledOnly)
        {
            return GetPluginDescriptors(loadMode)
                .SingleOrDefault(p => p.SystemName.Equals(systemName, StringComparison.InvariantCultureIgnoreCase));
        }

        /// <summary>
        /// Get a plugin descriptor by its system name
        /// </summary>
        /// <typeparam name="T">The type of plugin to get.</typeparam>
        /// <param name="systemName">Plugin system name</param>
        /// <param name="loadMode">Load plugins mode</param>
        /// <returns>>Plugin descriptor</returns>
        public virtual PluginDescriptor GetPluginDescriptorBySystemName<T>(string systemName, LoadPluginsMode loadMode = LoadPluginsMode.InstalledOnly)
            where T : class, IPlugin
        {
            return GetPluginDescriptors<T>(loadMode)
                .SingleOrDefault(p => p.SystemName.Equals(systemName, StringComparison.InvariantCultureIgnoreCase));
        }

        /// <summary>
        /// Reload plugins after updating
        /// </summary>
        /// <param name="pluginDescriptor">Updated plugin descriptor</param>
        public virtual void ReloadPlugins(PluginDescriptor pluginDescriptor)
        {
            _arePluginsLoaded = false;
            EnsurePluginsAreLoaded();

            //raise event
            _eventPublisher.Publish(new PluginUpdatedEvent(pluginDescriptor));
        }

        /// <summary>
        /// Find a plugin descriptor by some type which is located into the same assembly as plugin
        /// </summary>
        /// <param name="typeInAssembly">Type</param>
        /// <returns>Plugin descriptor if exists; otherwise null</returns>
        public virtual PluginDescriptor FindPlugin(Type typeInAssembly)
        {
            if (typeInAssembly == null)
                throw new ArgumentNullException(nameof(typeInAssembly));

            return PluginManager.ReferencedPlugins?.FirstOrDefault(plugin =>
                plugin.ReferencedAssembly != null &&
                plugin.ReferencedAssembly.FullName.Equals(typeInAssembly.Assembly.FullName, StringComparison.InvariantCultureIgnoreCase));
        }

        public virtual void InstallPlugins()
        {
            foreach (var installedPlugin in this.GetLastInstalledPlugins)
            {
                var customer = (installedPlugin.CustomerGuid.HasValue ? _customerService.GetCustomerByGuid(installedPlugin.CustomerGuid.Value) : null) ?? _workContext.CurrentCustomer;
                _customerActivityService.InsertActivity(customer, "InstallNewPlugin", string.Format(_localizationService.GetResource("ActivityLog.InstallNewPlugin"), installedPlugin.SystemName));
            }

            //log plugin installation errors
            foreach (var descriptor in PluginManager.ReferencedPlugins.Where(pluginDescriptor => pluginDescriptor.Error != null))
            {
                _logger.Error(string.Format(_localizationService.GetResource("ActivityLog.NotInstalledNewPluginError"), descriptor.SystemName), descriptor.Error);
                descriptor.Error = null;
            }
        }

        public virtual void UninstallPlugins()
        {
            //uninstall plugins
            foreach (var uninstalledPluginSystemName in this.UninstallPluginsIfNeed())
            {
                _customerActivityService.InsertActivity("UninstallPlugin", string.Format(_localizationService.GetResource("ActivityLog.UninstallPlugin"), uninstalledPluginSystemName));
            }

            //log plugin uninstallation errors
            foreach (var descriptor in PluginManager.ReferencedPlugins.Where(pluginDescriptor => pluginDescriptor.Error != null))
            {
                _logger.Error(string.Format(_localizationService.GetResource("ActivityLog.NotUninstalledPluginError"), descriptor.SystemName), descriptor.Error);
                descriptor.Error = null;
            }

        }

        public virtual void DeletePlugins()
        {
            //delete plugins
            foreach (var deletedPluginSystemName in this.DeletePluginsIfNeed())
            {
                _customerActivityService.InsertActivity("DeletePlugin", string.Format(_localizationService.GetResource("ActivityLog.DeletePlugin"), deletedPluginSystemName));
            }

            //log plugin deletion errors
            foreach (var descriptor in PluginManager.ReferencedPlugins.Where(pluginDescriptor => pluginDescriptor.Error != null))
            {
                _logger.Error(string.Format(_localizationService.GetResource("ActivityLog.NotDeletedPluginError"), descriptor.SystemName), descriptor.Error);
                descriptor.Error = null;
            }
        }

        /// <summary>
        /// Gets a list of system names of last time installed plugins 
        /// </summary>
        public virtual IReadOnlyCollection<PluginsInfo.PluginToInstall> GetLastInstalledPlugins
        {
            get
            {
                if (!_lastInstalledPluginSystemNames?.Any() ?? true)
                    InstallPluginsIfNeed();

                return _lastInstalledPluginSystemNames;
            }
        }

        #endregion
    }
}
