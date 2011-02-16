using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nop.Core.Infrastructure;

namespace Nop.Core.Plugins
{
    /// <summary>
    /// Finds plugins and calls their initializer.
    /// </summary>
    public interface IPluginBootstrapper
    {
        IEnumerable<IPluginDefinition> GetPluginDefinitions();
        void InitializePlugins(IEngine engine, IEnumerable<IPluginDefinition> plugins);
    }
}
