using Nop.Plugin.Misc.MailChimp.Data;

namespace Nop.Plugin.Misc.MailChimp.Services {
    public interface ISubscriptionEventQueueingService {
        /// <summary>
        /// Enqueues the specified record.
        /// </summary>
        /// <param name="record">The record.</param>
        void Enqueue(MailChimpEventQueueRecord record);

        /// <summary>
        /// Dequeues this instance.
        /// </summary>
        /// <returns></returns>
        MailChimpEventQueueRecord Dequeue();

        /// <summary>
        /// Queues all subscriptions.
        /// </summary>
        void QueueAll();
    }
}