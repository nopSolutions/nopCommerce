using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nop.Core.Infrastructure;
using Nop.Core.Plugins;

namespace Nop.Core.Tests.Plugin
{
    public class PlugInInitializer3 : IPluginInitializer
    {
        public static bool WasInitialized { get; set; }

        public void Initialize(IEngine engine)
        {
            WasInitialized = true;
        }
    }
}
