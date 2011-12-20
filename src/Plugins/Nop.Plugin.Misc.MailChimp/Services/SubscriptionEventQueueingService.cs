using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core.Data;
using Nop.Plugin.Misc.MailChimp.Data;

namespace Nop.Plugin.Misc.MailChimp.Services
{
    public class SubscriptionEventQueueingService : ISubscriptionEventQueueingService
    {
        private const string QUEUE_ALL_RECORDS = "INSERT INTO MailChimpEventQueueRecord (Email, IsSubscribe, CreatedOnUtc) SELECT Email, 1, getutcdate() FROM NewsLetterSubscription WHERE Active = 1";

        private readonly IRepository<MailChimpEventQueueRecord> _repository;
        private readonly MailChimpObjectContext _context;

        public SubscriptionEventQueueingService(IRepository<MailChimpEventQueueRecord> repository, MailChimpObjectContext context)
        {
            _repository = repository;
            _context = context;
        }

        /// <summary>
        /// Deletes the specified record.
        /// </summary>
        /// <param name="record">The record.</param>
        public virtual void Delete(MailChimpEventQueueRecord record)
        {
            if (record == null)
                throw new ArgumentNullException("record");

            _repository.Delete(record);
        }

        /// <summary>
        /// Inserts the specified record.
        /// </summary>
        /// <param name="record">The record.</param>
        public virtual void Insert(MailChimpEventQueueRecord record)
        {
            if (record == null)
                throw new ArgumentNullException("record");

            _repository.Insert(record);
        }
        
        /// <summary>
        /// Queues all subscriptions.
        /// </summary>
        public virtual void QueueAll()
        {
            //NOTE: While I dislike executing straight SQL from C#, I think straight SQL is the best solution for this particular action.
            // My logic is that #1 this query takes no arguments, it is a batch query and #2 this is potentially a very big operation.
            // Loading all of the NewsLetterSubscriptions, iterating them, converting them, and submitting the changes just seems like we would not be taking advantage of our tools (SQLServer).
            _context.Database.ExecuteSqlCommand(QUEUE_ALL_RECORDS);
            _context.SaveChanges();
        }

        /// <summary>
        /// Dequeues the list.
        /// </summary>
        /// <returns></returns>
        public virtual IList<MailChimpEventQueueRecord> GetAll()
        {
            var query = from r in _repository.Table
                        orderby r.CreatedOnUtc descending 
                        select r;

            return query.ToList();
        }
    }
}