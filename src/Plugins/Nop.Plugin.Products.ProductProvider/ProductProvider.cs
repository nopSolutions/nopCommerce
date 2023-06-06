using System;
using System.Threading.Tasks;
using Nop.Services.Plugins;

namespace Nop.Plugin.ProductProvider;

public class ProductProvider : BasePlugin, IPlugin
{
    public string GetConfigurationPageUrl()
    {
        throw new NotImplementedException();
    }

    public PluginDescriptor PluginDescriptor { get; set; }
    public Task InstallAsync()
    {
        return Task.CompletedTask;
    }

    public Task UninstallAsync()
    {
        return Task.CompletedTask;
    }

    public Task UpdateAsync(string currentVersion, string targetVersion)
    {
        return Task.CompletedTask;
    }

    public Task PreparePluginToUninstallAsync()
    {
        return Task.CompletedTask;
    }
}