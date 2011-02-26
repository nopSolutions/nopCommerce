using Nop.Core.Infrastructure;
using Nop.Core.Plugins;
using Nop.Core.Tests.Plugin;

[assembly: PluginInitializer("Testplugin", "testplugin", typeof(PlugInInitializer1))]
namespace Nop.Core.Tests.Plugin
{
    public class PlugInInitializer1 : IPluginInitializer
    {
        public static bool WasInitialized { get; set; }

        public void Initialize(IEngine engine)
        {
            WasInitialized = true;
        }
    }
}
