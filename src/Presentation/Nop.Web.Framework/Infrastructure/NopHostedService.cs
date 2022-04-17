using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Nop.Core.Configuration;
using Nop.Services.Plugins;
using Nop.Services.Security;

namespace Nop.Web.Framework.Infrastructure
{
    public class NopHostedService : IHostedService
    {
        private readonly AppSettings _appSettings;
        private readonly IPluginService _pluginService;
        private readonly IPermissionService _permissionService;

        public NopHostedService(IPluginService pluginService, AppSettings appSettings, IPermissionService permissionService)
        {
            _pluginService = pluginService;
            _appSettings = appSettings;
            _permissionService = permissionService;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await InstallPluginsAsync();
            await RegisterDefaultPermissionAsync();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private async Task InstallPluginsAsync()
        {
            var pluginsIgnoredDuringInstallation = new List<string>();

            if (!string.IsNullOrEmpty(_appSettings.Get<InstallationConfig>().DisabledPlugins))
            {
                pluginsIgnoredDuringInstallation = _appSettings
                    .Get<InstallationConfig>()
                    .DisabledPlugins
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(pluginName => pluginName.Trim())
                    .ToList();
            }

            var plugins = (await _pluginService.GetPluginDescriptorsAsync<IPlugin>(LoadPluginsMode.All))
                .Where(pluginDescriptor => !pluginsIgnoredDuringInstallation.Contains(pluginDescriptor.SystemName))
                .OrderBy(pluginDescriptor => pluginDescriptor.Group).ThenBy(pluginDescriptor => pluginDescriptor.DisplayOrder)
                .ToList();

            foreach (var plugin in plugins)
            {
                await _pluginService.PreparePluginToInstallAsync(plugin.SystemName, checkDependencies: false);
            }
        }

        private async Task RegisterDefaultPermissionAsync()
        {
            var permissionProviders = new List<Type> { typeof(StandardPermissionProvider) };
            foreach (var providerType in permissionProviders)
            {
                var provider = (IPermissionProvider)Activator.CreateInstance(providerType);

                await _permissionService.InstallPermissionsAsync(provider);
            }
        }
    }
}