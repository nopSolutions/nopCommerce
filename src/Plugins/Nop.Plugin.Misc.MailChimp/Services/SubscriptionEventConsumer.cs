using Nop.Core.Events;
using Nop.Plugin.Misc.MailChimp.Data;

namespace Nop.Plugin.Misc.MailChimp.Services {
    public class SubscriptionEventConsumer : IConsumer<EmailSubscribed>, IConsumer<EmailUnsubscribed> {
        private readonly ISubscriptionEventQueueingService _service;

        public SubscriptionEventConsumer(ISubscriptionEventQueueingService service) {
            _service = service;
        }

        #region Implementation of IConsumer<EmailSubscribed>

        /// <summary>
        /// Handles the event.
        /// </summary>
        /// <param name="eventMessage">The event message.</param>
        public void HandleEvent(EmailSubscribed eventMessage) {
            _service.Enqueue(new MailChimpEventQueueRecord {Email = eventMessage.Email, IsSubscribe = true});
        }

        #endregion

        #region Implementation of IConsumer<EmailUnsubscribed>

        /// <summary>
        /// Handles the event.
        /// </summary>
        /// <param name="eventMessage">The event message.</param>
        public void HandleEvent(EmailUnsubscribed eventMessage) {
            _service.Enqueue(new MailChimpEventQueueRecord {Email = eventMessage.Email, IsSubscribe = false});
        }

        #endregion
    }
}