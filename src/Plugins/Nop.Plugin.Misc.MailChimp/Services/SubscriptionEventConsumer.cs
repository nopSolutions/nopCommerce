using Nop.Core.Events;
using Nop.Core.Plugins;
using Nop.Plugin.Misc.MailChimp.Data;

namespace Nop.Plugin.Misc.MailChimp.Services
{
    public class SubscriptionEventConsumer : IConsumer<EmailSubscribed>, IConsumer<EmailUnsubscribed>
    {
        private readonly ISubscriptionEventQueueingService _service;
        private readonly IPluginFinder _pluginFinder;

        public SubscriptionEventConsumer(ISubscriptionEventQueueingService service,
            IPluginFinder pluginFinder)
        {
            this._service = service;
            this._pluginFinder = pluginFinder;
        }

        /// <summary>
        /// Handles the event.
        /// </summary>
        /// <param name="eventMessage">The event message.</param>
        public  void HandleEvent(EmailSubscribed eventMessage)
        {
            //is plugin installed?
            var pluginDescriptor = _pluginFinder.GetPluginDescriptorBySystemName("Misc.MailChimp");
            if (pluginDescriptor == null || !pluginDescriptor.Installed)
                return;
            
            _service.Insert(new MailChimpEventQueueRecord { Email = eventMessage.Email, IsSubscribe = true });
        }

        /// <summary>
        /// Handles the event.
        /// </summary>
        /// <param name="eventMessage">The event message.</param>
        public void HandleEvent(EmailUnsubscribed eventMessage)
        {
            //is plugin installed?
            var pluginDescriptor = _pluginFinder.GetPluginDescriptorBySystemName("Misc.MailChimp");
            if (pluginDescriptor == null || !pluginDescriptor.Installed)
                return;

            _service.Insert(new MailChimpEventQueueRecord { Email = eventMessage.Email, IsSubscribe = false });
        }
    }
}