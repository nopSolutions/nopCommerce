using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;

namespace Nop.Core.Plugins
{
    public interface IPluginFinder
    {
        /// <summary>Gets plugins found in the environment sorted.</summary>
        /// <typeparam name="T">The type of plugin to get.</typeparam>
        /// <returns>An enumeration of plugins.</returns>
        IEnumerable<T> GetPlugins<T>() where T : class, IPlugin;

        IEnumerable<PluginDescriptor> GetPluginDescriptors();

        IEnumerable<PluginDescriptor> GetPluginDescriptors<T>() where T : class, IPlugin;
    }
}
