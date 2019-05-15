using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Infrastructure;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Logging;

namespace Nop.Services.Plugins
{
    /// <summary>
    /// Represents the plugin service implementation
    /// </summary>
    public partial class PluginService : IPluginService
    {
        #region Fields

        private readonly CatalogSettings _catalogSettings;
        private readonly ICustomerService _customerService;
        private readonly ILogger _logger;
        private readonly INopFileProvider _fileProvider;
        private readonly IWebHelper _webHelper;
        private readonly IPluginsInfo _pluginsInfo;

        #endregion

        #region Ctor

        public PluginService(CatalogSettings catalogSettings,
            ICustomerService customerService,
            ILogger logger,
            INopFileProvider fileProvider,
            IWebHelper webHelper)
        {
            _catalogSettings = catalogSettings;
            _customerService = customerService;
            _logger = logger;
            _fileProvider = fileProvider;
            _webHelper = webHelper;
            _pluginsInfo = Singleton<IPluginsInfo>.Instance;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Check whether to load the plugin based on the load mode passed
        /// </summary>
        /// <param name="pluginDescriptor">Plugin descriptor to check</param>
        /// <param name="loadMode">Load plugins mode</param>
        /// <returns>Result of check</returns>
        protected virtual bool FilterByLoadMode(PluginDescriptor pluginDescriptor, LoadPluginsMode loadMode)
        {
            if (pluginDescriptor == null)
                throw new ArgumentNullException(nameof(pluginDescriptor));

            switch (loadMode)
            {
                case LoadPluginsMode.All:
                    return true;

                case LoadPluginsMode.InstalledOnly:
                    return pluginDescriptor.Installed;

                case LoadPluginsMode.NotInstalledOnly:
                    return !pluginDescriptor.Installed;

                default:
                    throw new NotSupportedException(nameof(loadMode));
            }
        }

        /// <summary>
        /// Check whether to load the plugin based on the plugin group passed
        /// </summary>
        /// <param name="pluginDescriptor">Plugin descriptor to check</param>
        /// <param name="group">Group name</param>
        /// <returns>Result of check</returns>
        protected virtual bool FilterByPluginGroup(PluginDescriptor pluginDescriptor, string group)
        {
            if (pluginDescriptor == null)
                throw new ArgumentNullException(nameof(pluginDescriptor));

            if (string.IsNullOrEmpty(group))
                return true;

            return group.Equals(pluginDescriptor.Group, StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Check whether to load the plugin based on the customer passed
        /// </summary>
        /// <param name="pluginDescriptor">Plugin descriptor to check</param>
        /// <param name="customer">Customer</param>
        /// <returns>Result of check</returns>
        protected virtual bool FilterByCustomer(PluginDescriptor pluginDescriptor, Customer customer)
        {
            if (pluginDescriptor == null)
                throw new ArgumentNullException(nameof(pluginDescriptor));

            if (customer == null || !pluginDescriptor.LimitedToCustomerRoles.Any())
                return true;

            if (_catalogSettings.IgnoreAcl)
                return true;

            return pluginDescriptor.LimitedToCustomerRoles.Intersect(customer.GetCustomerRoleIds()).Any();
        }

        /// <summary>
        /// Check whether to load the plugin based on the store identifier passed
        /// </summary>
        /// <param name="pluginDescriptor">Plugin descriptor to check</param>
        /// <param name="storeId">Store identifier</param>
        /// <returns>Result of check</returns>
        protected virtual bool FilterByStore(PluginDescriptor pluginDescriptor, int storeId)
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

        #endregion

        #region Methods

        /// <summary>
        /// Get plugin descriptors
        /// </summary>
        /// <typeparam name="TPlugin">The type of plugins to get</typeparam>
        /// <param name="loadMode">Filter by load plugins mode</param>
        /// <param name="customer">Filter by  customer; pass null to load all records</param>
        /// <param name="storeId">Filter by store; pass 0 to load all records</param>
        /// <param name="group">Filter by plugin group; pass null to load all records</param>
        /// <returns>Plugin descriptors</returns>
        public virtual IEnumerable<PluginDescriptor> GetPluginDescriptors<TPlugin>(LoadPluginsMode loadMode = LoadPluginsMode.InstalledOnly,
            Customer customer = null, int storeId = 0, string group = null) where TPlugin : class, IPlugin
        {
            var pluginDescriptors = _pluginsInfo.PluginDescriptors;

            //filter plugins
            pluginDescriptors = pluginDescriptors.Where(descriptor =>
                FilterByLoadMode(descriptor, loadMode) &&
                FilterByCustomer(descriptor, customer) &&
                FilterByStore(descriptor, storeId) &&
                FilterByPluginGroup(descriptor, group));

            //filter by the passed type
            if (typeof(TPlugin) != typeof(IPlugin))
            {
                pluginDescriptors = pluginDescriptors
                    .Where(descriptor => typeof(TPlugin).IsAssignableFrom(descriptor.PluginType))
                    .OrderBy(descriptor => descriptor.DisplayOrder);
            }
            else
                pluginDescriptors = pluginDescriptors.OrderBy(descriptor => descriptor.Group);

            return pluginDescriptors;
        }

        /// <summary>
        /// Get a plugin descriptor by the system name
        /// </summary>
        /// <typeparam name="TPlugin">The type of plugin to get</typeparam>
        /// <param name="systemName">Plugin system name</param>
        /// <param name="loadMode">Load plugins mode</param>
        /// <param name="customer">Filter by  customer; pass null to load all records</param>
        /// <param name="storeId">Filter by store; pass 0 to load all records</param>
        /// <param name="group">Filter by plugin group; pass null to load all records</param>
        /// <returns>>Plugin descriptor</returns>
        public virtual PluginDescriptor GetPluginDescriptorBySystemName<TPlugin>(string systemName,
            LoadPluginsMode loadMode = LoadPluginsMode.InstalledOnly,
            Customer customer = null, int storeId = 0, string group = null) where TPlugin : class, IPlugin
        {
            return GetPluginDescriptors<TPlugin>(loadMode, customer, storeId, group)
                .FirstOrDefault(descriptor => descriptor.SystemName.Equals(systemName, StringComparison.InvariantCultureIgnoreCase));
        }

        /// <summary>
        /// Get plugins
        /// </summary>
        /// <typeparam name="TPlugin">The type of plugins to get</typeparam>
        /// <param name="loadMode">Filter by load plugins mode</param>
        /// <param name="customer">Filter by customer; pass null to load all records</param>
        /// <param name="storeId">Filter by store; pass 0 to load all records</param>
        /// <param name="group">Filter by plugin group; pass null to load all records</param>
        /// <returns>Plugins</returns>
        public virtual IEnumerable<TPlugin> GetPlugins<TPlugin>(LoadPluginsMode loadMode = LoadPluginsMode.InstalledOnly,
            Customer customer = null, int storeId = 0, string group = null) where TPlugin : class, IPlugin
        {
            return GetPluginDescriptors<TPlugin>(loadMode, customer, storeId, group)
                .Select(descriptor => descriptor.Instance<TPlugin>());
        }

        /// <summary>
        /// Find a plugin by the type which is located into the same assembly as a plugin
        /// </summary>
        /// <param name="typeInAssembly">Type</param>
        /// <returns>Plugin</returns>
        public virtual IPlugin FindPluginByTypeInAssembly(Type typeInAssembly)
        {
            if (typeInAssembly == null)
                throw new ArgumentNullException(nameof(typeInAssembly));

            //try to do magic
            var pluginDescriptor = _pluginsInfo.PluginDescriptors.FirstOrDefault(descriptor =>
               descriptor.ReferencedAssembly?.FullName.Equals(typeInAssembly.Assembly.FullName, StringComparison.InvariantCultureIgnoreCase) ?? false);

            return pluginDescriptor?.Instance<IPlugin>();
        }

        /// <summary>
        /// Get plugin logo URL
        /// </summary>
        /// <param name="pluginDescriptor">Plugin descriptor</param>
        /// <returns>Logo URL</returns>
        public virtual string GetPluginLogoUrl(PluginDescriptor pluginDescriptor)
        {
            var pluginDirectory = _fileProvider.GetDirectoryName(pluginDescriptor.OriginalAssemblyFile);
            if (string.IsNullOrEmpty(pluginDirectory))
                return null;

            //check for supported extensions
            var logoExtension = NopPluginDefaults.SupportedLogoImageExtensions
                .FirstOrDefault(ext => _fileProvider.FileExists(_fileProvider.Combine(pluginDirectory, $"{NopPluginDefaults.LogoFileName}.{ext}")));
            if (string.IsNullOrWhiteSpace(logoExtension))
                return null;

            var storeLocation = _webHelper.GetStoreLocation();
            var logoUrl = $"{storeLocation}{NopPluginDefaults.PathName}/" +
                $"{_fileProvider.GetDirectoryNameOnly(pluginDirectory)}/{NopPluginDefaults.LogoFileName}.{logoExtension}";

            return logoUrl;
        }

        /// <summary>
        /// Prepare plugin to the installation
        /// </summary>
        /// <param name="systemName">Plugin system name</param>
        /// <param name="customer">Customer</param>
        public virtual void PreparePluginToInstall(string systemName, Customer customer = null)
        {
            //add plugin name to the appropriate list (if not yet contained) and save changes
            if (_pluginsInfo.PluginNamesToInstall.Any(item => item.SystemName == systemName))
                return;

            _pluginsInfo.PluginNamesToInstall.Add((systemName, customer?.CustomerGuid));
            _pluginsInfo.Save();
        }

        /// <summary>
        /// Prepare plugin to the uninstallation
        /// </summary>
        /// <param name="systemName">Plugin system name</param>
        public virtual void PreparePluginToUninstall(string systemName)
        {
            //add plugin name to the appropriate list (if not yet contained) and save changes
            if (_pluginsInfo.PluginNamesToUninstall.Contains(systemName))
                return;

            var plugin = GetPluginDescriptorBySystemName<IPlugin>(systemName)?.Instance<IPlugin>();
            plugin?.PreparePluginToUninstall();

            _pluginsInfo.PluginNamesToUninstall.Add(systemName);
            _pluginsInfo.Save();
        }

        /// <summary>
        /// Prepare plugin to the removing
        /// </summary>
        /// <param name="systemName">Plugin system name</param>
        public virtual void PreparePluginToDelete(string systemName)
        {
            //add plugin name to the appropriate list (if not yet contained) and save changes
            if (_pluginsInfo.PluginNamesToDelete.Contains(systemName))
                return;

            _pluginsInfo.PluginNamesToDelete.Add(systemName);
            _pluginsInfo.Save();
        }

        /// <summary>
        /// Reset changes
        /// </summary>
        public virtual void ResetChanges()
        {
            //clear lists and save changes
            _pluginsInfo.PluginNamesToDelete.Clear();
            _pluginsInfo.PluginNamesToInstall.Clear();
            _pluginsInfo.PluginNamesToUninstall.Clear();
            _pluginsInfo.Save();

            //display all plugins on the plugin list page
            _pluginsInfo.PluginDescriptors.ToList().ForEach(pluginDescriptor => pluginDescriptor.ShowInPluginsList = true);
        }

        /// <summary>
        /// Clear installed plugins list
        /// </summary>
        public virtual void ClearInstalledPluginsList()
        {
            _pluginsInfo.InstalledPluginNames.Clear();
        }

        /// <summary>
        /// Install plugins
        /// </summary>
        public virtual void InstallPlugins()
        {
            //get all uninstalled plugins
            var pluginDescriptors = _pluginsInfo.PluginDescriptors.Where(descriptor => !descriptor.Installed).ToList();

            //filter plugins need to install
            pluginDescriptors = pluginDescriptors.Where(descriptor => _pluginsInfo.PluginNamesToInstall
                .Any(item => item.SystemName.Equals(descriptor.SystemName))).ToList();
            if (!pluginDescriptors.Any())
                return;

            //do not inject services via constructor because it'll cause circular references
            var localizationService = EngineContext.Current.Resolve<ILocalizationService>();
            var customerActivityService = EngineContext.Current.Resolve<ICustomerActivityService>();

            //install plugins
            foreach (var descriptor in pluginDescriptors)
            {
                try
                {
                    //try to install an instance
                    descriptor.Instance<IPlugin>().Install();

                    //remove and add plugin system name to appropriate lists
                    var pluginToInstall = _pluginsInfo.PluginNamesToInstall
                        .FirstOrDefault(plugin => plugin.SystemName.Equals(descriptor.SystemName));
                    _pluginsInfo.InstalledPluginNames.Add(descriptor.SystemName);
                    _pluginsInfo.PluginNamesToInstall.Remove(pluginToInstall);

                    //activity log
                    var customer = _customerService.GetCustomerByGuid(pluginToInstall.CustomerGuid ?? Guid.Empty);
                    customerActivityService.InsertActivity(customer, "InstallNewPlugin",
                        string.Format(localizationService.GetResource("ActivityLog.InstallNewPlugin"), descriptor.SystemName));

                    //mark the plugin as installed
                    descriptor.Installed = true;
                    descriptor.ShowInPluginsList = true;
                }
                catch (Exception exception)
                {
                    //log error
                    var message = string.Format(localizationService.GetResource("Admin.Plugins.Errors.NotInstalled"), descriptor.SystemName);
                    _logger.Error(message, exception);
                }
            }

            //save changes
            _pluginsInfo.Save();
        }

        /// <summary>
        /// Uninstall plugins
        /// </summary>
        public virtual void UninstallPlugins()
        {
            //get all installed plugins
            var pluginDescriptors = _pluginsInfo.PluginDescriptors.Where(descriptor => descriptor.Installed).ToList();

            //filter plugins need to uninstall
            pluginDescriptors = pluginDescriptors
                .Where(descriptor => _pluginsInfo.PluginNamesToUninstall.Contains(descriptor.SystemName)).ToList();
            if (!pluginDescriptors.Any())
                return;

            //do not inject services via constructor because it'll cause circular references
            var localizationService = EngineContext.Current.Resolve<ILocalizationService>();
            var customerActivityService = EngineContext.Current.Resolve<ICustomerActivityService>();

            //uninstall plugins
            foreach (var descriptor in pluginDescriptors)
            {
                try
                {
                    //try to uninstall an instance
                    descriptor.Instance<IPlugin>().Uninstall();

                    //remove plugin system name from appropriate lists
                    _pluginsInfo.InstalledPluginNames.Remove(descriptor.SystemName);
                    _pluginsInfo.PluginNamesToUninstall.Remove(descriptor.SystemName);

                    //activity log
                    customerActivityService.InsertActivity("UninstallPlugin",
                        string.Format(localizationService.GetResource("ActivityLog.UninstallPlugin"), descriptor.SystemName));

                    //mark the plugin as uninstalled
                    descriptor.Installed = false;
                    descriptor.ShowInPluginsList = true;
                }
                catch (Exception exception)
                {
                    //log error
                    var message = string.Format(localizationService.GetResource("Admin.Plugins.Errors.NotUninstalled"), descriptor.SystemName);
                    _logger.Error(message, exception);
                }
            }

            //save changes
            _pluginsInfo.Save();
        }

        /// <summary>
        /// Delete plugins
        /// </summary>
        public virtual void DeletePlugins()
        {
            //get all uninstalled plugins (delete plugin only previously uninstalled)
            var pluginDescriptors = _pluginsInfo.PluginDescriptors.Where(descriptor => !descriptor.Installed).ToList();

            //filter plugins need to delete
            pluginDescriptors = pluginDescriptors
                .Where(descriptor => _pluginsInfo.PluginNamesToDelete.Contains(descriptor.SystemName)).ToList();
            if (!pluginDescriptors.Any())
                return;

            //do not inject services via constructor because it'll cause circular references
            var localizationService = EngineContext.Current.Resolve<ILocalizationService>();
            var customerActivityService = EngineContext.Current.Resolve<ICustomerActivityService>();

            //delete plugins
            foreach (var descriptor in pluginDescriptors)
            {
                try
                {
                    //try to delete a plugin directory from disk storage
                    var pluginDirectory = _fileProvider.GetDirectoryName(descriptor.OriginalAssemblyFile);
                    if (_fileProvider.DirectoryExists(pluginDirectory))
                        _fileProvider.DeleteDirectory(pluginDirectory);

                    //remove plugin system name from the appropriate list
                    _pluginsInfo.PluginNamesToDelete.Remove(descriptor.SystemName);

                    //activity log
                    customerActivityService.InsertActivity("DeletePlugin",
                        string.Format(localizationService.GetResource("ActivityLog.DeletePlugin"), descriptor.SystemName));
                }
                catch (Exception exception)
                {
                    //log error
                    var message = string.Format(localizationService.GetResource("Admin.Plugins.Errors.NotDeleted"), descriptor.SystemName);
                    _logger.Error(message, exception);
                }
            }

            //save changes
            _pluginsInfo.Save();
        }

        /// <summary>
        /// Check whether application restart is required to apply changes to plugins
        /// </summary>
        /// <returns>Result of check</returns>
        public virtual bool IsRestartRequired()
        {
            //return true if any of lists contains items
            return _pluginsInfo.PluginNamesToInstall.Any()
                || _pluginsInfo.PluginNamesToUninstall.Any()
                || _pluginsInfo.PluginNamesToDelete.Any();
        }

        /// <summary>
        /// Get names of incompatible plugins
        /// </summary>
        /// <returns>List of plugin names</returns>
        public virtual IList<string> GetIncompatiblePlugins()
        {
            return _pluginsInfo.IncompatiblePlugins;
        }

        /// <summary>
        /// Get all assembly loaded collisions
        /// </summary>
        /// <returns>List of plugin loaded assembly info</returns>
        public virtual IList<PluginLoadedAssemblyInfo> GetAssemblyCollisions()
        {
            return _pluginsInfo.AssemblyLoadedCollision;
        }

        #endregion
    }
}