using System;
using System.Threading.Tasks;
using Nop.Services.Plugins;

namespace Nop.Plugin.ProductProvider;

public class ProductProvider : IPlugin
{
    public string GetConfigurationPageUrl()
    {
        throw new NotImplementedException();
    }

    public PluginDescriptor PluginDescriptor { get; set; }
    public Task InstallAsync()
    {
        throw new NotImplementedException();
    }

    public Task UninstallAsync()
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(string currentVersion, string targetVersion)
    {
        throw new NotImplementedException();
    }

    public Task PreparePluginToUninstallAsync()
    {
        throw new NotImplementedException();
    }
}