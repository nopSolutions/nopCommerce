using Nop.Services.Plugins;
using Nop.Services.Common;
using System.Threading.Tasks;

namespace Nop.Plugin.Api.Rest
{
    public class Plugin : BasePlugin, IMiscPlugin
    {
        public override async Task InstallAsync()
        {
            // add plugin settings or DB initialization here if needed
            await base.InstallAsync();
        }

        public override async Task UninstallAsync()
        {
            // cleanup plugin data if necessary
            await base.UninstallAsync();
        }
    }
}
