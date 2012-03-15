using Nop.Core.Infrastructure;
using Nop.Core.Plugins;
using Nop.Plugin.Misc.MailChimp.Services;
using Nop.Services.Tasks;

namespace Nop.Plugin.Misc.MailChimp
{
    public class MailChimpSynchronizationTask : ITask
    {
        /// <summary>
        /// Execute task
        /// </summary>
        public void Execute()
        {
            var pluginFinder = EngineContext.Current.Resolve<IPluginFinder>();

            //is plugin installed?
            var pluginDescriptor = pluginFinder.GetPluginDescriptorBySystemName("Misc.MailChimp");
            if (pluginDescriptor == null || !pluginDescriptor.Installed)
                return;

            //is plugin configured?
            var plugin = pluginDescriptor.Instance() as MailChimpPlugin;
            if (plugin == null || !plugin.IsConfigured())
                return;

            var mailChimpApiService = EngineContext.Current.Resolve<IMailChimpApiService>();
            mailChimpApiService.Synchronize();
        }
    }
}