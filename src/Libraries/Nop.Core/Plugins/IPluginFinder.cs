using System.Collections.Generic;

namespace Nop.Core.Plugins
{
    public interface IPluginFinder
    {
        /// <summary>Gets plugins found in the environment sorted.</summary>
        /// <typeparam name="T">The type of plugin to get.</typeparam>
        /// <returns>An enumeration of plugins.</returns>
        IEnumerable<T> GetPlugins<T>(bool installedOnly = true) where T : class, IPlugin;

        IEnumerable<PluginDescriptor> GetPluginDescriptors(bool installedOnly = true);

        IEnumerable<PluginDescriptor> GetPluginDescriptors<T>(bool installedOnly = true) where T : class, IPlugin;
    }
}
