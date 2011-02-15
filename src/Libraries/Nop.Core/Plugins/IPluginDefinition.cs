using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nop.Core.Infrastructure;

namespace Nop.Core.Plugins
{
    /// <summary>
    /// Classes implementing this interface define plugins and are responsible of
    /// calling plugin initializers.
    /// </summary>
    public interface IPluginDefinition
    {
        /// <summary>Executes the plugin initializer.</summary>
        /// <param name="engine">A reference to the current engine.</param>
        void Initialize(IEngine engine);
    }
}
