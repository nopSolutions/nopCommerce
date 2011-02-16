using Nop.Core.Infrastructure;
using Nop.Core.Plugins;
using Nop.Core.Tests.Plugin;

[assembly: Plugin("Testplugin", "testplugin", typeof(PlugIn1))]

namespace Nop.Core.Tests.Plugin
{
    public class PlugIn1 : IPluginInitializer
    {
        public static bool WasInitialized { get; set; }

        public void Initialize(IEngine engine)
        {
            WasInitialized = true;
        }
    }
}
