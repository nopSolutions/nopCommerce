using Nop.Core.Plugins;
using Nop.Plugin.Misc.MailChimp.Services;
using Nop.Services.Tasks;

namespace Nop.Plugin.Misc.MailChimp
{
    public class MailChimpSynchronizationTask : ITask
    {
        private readonly IPluginFinder _pluginFinder;
        private readonly IMailChimpApiService _mailChimpApiService;

        public MailChimpSynchronizationTask(IPluginFinder pluginFinder, IMailChimpApiService mailChimpApiService)
        {
            this._pluginFinder = pluginFinder;
            this._mailChimpApiService = mailChimpApiService;
        }

        /// <summary>
        /// Execute task
        /// </summary>
        public void Execute()
        {
            //is plugin installed?
            var pluginDescriptor = _pluginFinder.GetPluginDescriptorBySystemName("Misc.MailChimp");
            if (pluginDescriptor == null)
                return;

            //is plugin configured?
            var plugin = pluginDescriptor.Instance() as MailChimpPlugin;
            if (plugin == null || !plugin.IsConfigured())
                return;

            _mailChimpApiService.Synchronize();
        }
    }
}