using System;
using Nop.Core.Plugins;

namespace Nop.Services.Plugins
{
    public partial interface IPluginService
    {
        /// <summary>
        /// Find a plugin descriptor by some type which is located into the same assembly as plugin
        /// </summary>
        /// <param name="typeInAssembly">Type</param>
        /// <returns>Plugin descriptor if exists; otherwise null</returns>
        PluginDescriptor FindPlugin(Type typeInAssembly);

        void InstallPlugins();

        void UninstallPlugins();

        void DeletePlugins();
    }
}