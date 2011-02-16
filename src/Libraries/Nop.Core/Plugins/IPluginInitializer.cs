using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nop.Core.Infrastructure;

namespace Nop.Core.Plugins
{
    /// <summary>Classes implementing this interface can serve as plug in initializers. If one of these classes is referenced by a PlugInAttribute it's initialize methods will be invoked by the nop factory during initialization.</summary>
    public interface IPluginInitializer
    {
        /// <summary>Invoked after the factory has been initialized.</summary>
        /// <param name="engine">The factory that has been initialized.</param>
        void Initialize(IEngine engine);
    }
}
