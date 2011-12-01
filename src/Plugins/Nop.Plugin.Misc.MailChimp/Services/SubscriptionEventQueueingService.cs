using System.Linq;
using Nop.Core.Data;
using Nop.Plugin.Misc.MailChimp.Data;

namespace Nop.Plugin.Misc.MailChimp.Services {
    public class SubscriptionEventQueueingService : ISubscriptionEventQueueingService {
        private readonly IRepository<MailChimpEventQueueRecord> _repository;

        public SubscriptionEventQueueingService(IRepository<MailChimpEventQueueRecord> repository) {
            _repository = repository;
        }

        #region Implementation of ISubscriptionEventQueueingService

        /// <summary>
        /// Enqueues the specified record.
        /// </summary>
        /// <param name="record">The record.</param>
        public void Enqueue(MailChimpEventQueueRecord record) {
            _repository.Insert(record);
        }

        /// <summary>
        /// Dequeues this instance.
        /// </summary>
        /// <returns></returns>
        public MailChimpEventQueueRecord Dequeue() {
            var query = from r in _repository.Table
                        orderby r.Id
                        select r;

            MailChimpEventQueueRecord output = query.FirstOrDefault();

            _repository.Delete(output);

            return output;
        }

        #endregion
    }
}