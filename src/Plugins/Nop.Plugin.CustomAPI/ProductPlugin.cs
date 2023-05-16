using Nop.Services.Plugins;

namespace Nop.Plugin.CustomAPI
{
    public class ProductPlugin : BasePlugin 
    {

        public override async Task InstallAsync()
        {
            await base.InstallAsync();
        }

        public override async Task UninstallAsync()
        {

            await base.UninstallAsync();
        }
    }
}
