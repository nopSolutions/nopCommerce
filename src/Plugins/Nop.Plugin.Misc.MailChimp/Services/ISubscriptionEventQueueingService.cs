using System.Collections.Generic;
using Nop.Plugin.Misc.MailChimp.Data;

namespace Nop.Plugin.Misc.MailChimp.Services
{
    public interface ISubscriptionEventQueueingService
    {
        /// <summary>
        /// Deletes the specified record.
        /// </summary>
        /// <param name="record">The record.</param>
        void Delete(MailChimpEventQueueRecord record);

        /// <summary>
        /// Inserts the specified record.
        /// </summary>
        /// <param name="record">The record.</param>
        void Insert(MailChimpEventQueueRecord record);

        /// <summary>
        /// Queues all subscriptions.
        /// </summary>
        void QueueAll();

        /// <summary>
        /// Reads the list.
        /// </summary>
        /// <param name="subscribeOnly">if set to <c>true</c> [subscribe only].</param>
        /// <returns></returns>
        IList<MailChimpEventQueueRecord> ReadList(bool subscribeOnly);
    }
}