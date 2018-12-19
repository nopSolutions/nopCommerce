using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using Nop.Core.Plugins;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Logging;

namespace Nop.Services.Plugins
{
    public partial class PluginService : IPluginService
    {
        #region Fields

        private readonly ICustomerActivityService _customerActivityService;
        private readonly ICustomerService _customerService;
        private readonly ILocalizationService _localizationService;
        private readonly ILogger _logger;
        private readonly IWorkContext _workContext;

        private static List<PluginsInfo.PluginToInstall> _lastInstalledPluginSystemNames;
        
        #endregion

        #region Ctor

        public PluginService(ICustomerActivityService customerActivityService,
            ICustomerService customerService,
            ILocalizationService localizationService,
            ILogger logger,
            IWorkContext workContext)
        {
            this._customerActivityService = customerActivityService;
            this._customerService = customerService;
            this._localizationService = localizationService;
            this._logger = logger;
            this._workContext = workContext;
        }

        #endregion

        #region Utilities

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