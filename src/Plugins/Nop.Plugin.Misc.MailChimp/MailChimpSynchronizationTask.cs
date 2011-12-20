using Nop.Core.Infrastructure;
using Nop.Core.Plugins;
using Nop.Plugin.Misc.MailChimp.Services;
using Nop.Services.Tasks;

namespace Nop.Plugin.Misc.MailChimp
{
    public class MailChimpSynchronizationTask : ITask
    {
        private readonly IMailChimpApiService _mailChimpApiService = EngineContext.Current.Resolve<IMailChimpApiService>();
        private readonly IPluginFinder _pluginFinder = EngineContext.Current.Resolve<IPluginFinder>();

        /// <summary>
        /// Execute task
        /// </summary>
        public void Execute()
        {
            //is plugin installed?
            var pluginDescriptor = _pluginFinder.GetPluginDescriptorBySystemName("Misc.MailChimp");
            if (pluginDescriptor == null || !pluginDescriptor.Installed)
                return;

            //is plugin configured?
            var plugin = pluginDescriptor.Instance() as MailChimpPlugin;
            if (plugin == null || !plugin.IsConfigured())
                return;

            _mailChimpApiService.BatchSubscribe();
            _mailChimpApiService.BatchUnsubscribe();
        }
    }
}