using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Messages;
using Nop.Data;
using Nop.Data.Extensions;
using Nop.Services.Events;

namespace Nop.Services.Messages
{
    /// <summary>
    /// Queued email service
    /// </summary>
    public partial class QueuedEmailService : IQueuedEmailService
    {
        #region Fields

        private readonly IDbContext _dbContext;
        private readonly IEventPublisher _eventPublisher;
        private readonly IRepository<QueuedEmail> _queuedEmailRepository;

        #endregion

        #region Ctor

        public QueuedEmailService(IDbContext dbContext,
            IEventPublisher eventPublisher,
            IRepository<QueuedEmail> queuedEmailRepository)
        {
            _dbContext = dbContext;
            _eventPublisher = eventPublisher;
            _queuedEmailRepository = queuedEmailRepository;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Inserts a queued email
        /// </summary>
        /// <param name="queuedEmail">Queued email</param>        
        public virtual void InsertQueuedEmail(QueuedEmail queuedEmail)
        {
            if (queuedEmail == null)
                throw new ArgumentNullException(nameof(queuedEmail));

            _queuedEmailRepository.Insert(queuedEmail);

            //event notification
            _eventPublisher.EntityInserted(queuedEmail);
        }

        /// <summary>
        /// Updates a queued email
        /// </summary>
        /// <param name="queuedEmail">Queued email</param>
        public virtual void UpdateQueuedEmail(QueuedEmail queuedEmail)
        {
            if (queuedEmail == null)
                throw new ArgumentNullException(nameof(queuedEmail));

            _queuedEmailRepository.Update(queuedEmail);

            //event notification
            _eventPublisher.EntityUpdated(queuedEmail);
        }

        /// <summary>
        /// Deleted a queued email
        /// </summary>
        /// <param name="queuedEmail">Queued email</param>
        public virtual void DeleteQueuedEmail(QueuedEmail queuedEmail)
        {
            if (queuedEmail == null)
                throw new ArgumentNullException(nameof(queuedEmail));

            _queuedEmailRepository.Delete(queuedEmail);

            //event notification
            _eventPublisher.EntityDeleted(queuedEmail);
        }

        /// <summary>
        /// Deleted a queued emails
        /// </summary>
        /// <param name="queuedEmails">Queued emails</param>
        public virtual void DeleteQueuedEmails(IList<QueuedEmail> queuedEmails)
        {
            if (queuedEmails == null)
                throw new ArgumentNullException(nameof(queuedEmails));

            _queuedEmailRepository.Delete(queuedEmails);

            //event notification
            foreach (var queuedEmail in queuedEmails)
            {
                _eventPublisher.EntityDeleted(queuedEmail);
            }
        }

        /// <summary>
        /// Gets a queued email by identifier
        /// </summary>
        /// <param name="queuedEmailId">Queued email identifier</param>
        /// <returns>Queued email</returns>
        public virtual QueuedEmail GetQueuedEmailById(int queuedEmailId)
        {
            if (queuedEmailId == 0)
                return null;

            return _queuedEmailRepository.GetById(queuedEmailId);
        }

        /// <summary>
        /// Get queued emails by identifiers
        /// </summary>
        /// <param name="queuedEmailIds">queued email identifiers</param>
        /// <returns>Queued emails</returns>
        public virtual IList<QueuedEmail> GetQueuedEmailsByIds(int[] queuedEmailIds)
        {
            if (queuedEmailIds == null || queuedEmailIds.Length == 0)
                return new List<QueuedEmail>();

            var query = from qe in _queuedEmailRepository.Table
                        where queuedEmailIds.Contains(qe.Id)
                        select qe;
            var queuedEmails = query.ToList();
            //sort by passed identifiers
            var sortedQueuedEmails = new List<QueuedEmail>();
            foreach (var id in queuedEmailIds)
            {
                var queuedEmail = queuedEmails.Find(x => x.Id == id);
                if (queuedEmail != null)
                    sortedQueuedEmails.Add(queuedEmail);
            }

            return sortedQueuedEmails;
        }

        /// <summary>
        /// Gets all queued emails
        /// </summary>
        /// <param name="fromEmail">From Email</param>
        /// <param name="toEmail">To Email</param>
        /// <param name="createdFromUtc">Created date from (UTC); null to load all records</param>
        /// <param name="createdToUtc">Created date to (UTC); null to load all records</param>
        /// <param name="loadNotSentItemsOnly">A value indicating whether to load only not sent emails</param>
        /// <param name="loadOnlyItemsToBeSent">A value indicating whether to load only emails for ready to be sent</param>
        /// <param name="maxSendTries">Maximum send tries</param>
        /// <param name="loadNewest">A value indicating whether we should sort queued email descending; otherwise, ascending.</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Email item list</returns>
        public virtual IPagedList<QueuedEmail> SearchEmails(string fromEmail,
            string toEmail, DateTime? createdFromUtc, DateTime? createdToUtc,
            bool loadNotSentItemsOnly, bool loadOnlyItemsToBeSent, int maxSendTries,
            bool loadNewest, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            fromEmail = (fromEmail ?? string.Empty).Trim();
            toEmail = (toEmail ?? string.Empty).Trim();

            var query = _queuedEmailRepository.Table;
            if (!string.IsNullOrEmpty(fromEmail))
                query = query.Where(qe => qe.From.Contains(fromEmail));
            if (!string.IsNullOrEmpty(toEmail))
                query = query.Where(qe => qe.To.Contains(toEmail));
            if (createdFromUtc.HasValue)
                query = query.Where(qe => qe.CreatedOnUtc >= createdFromUtc);
            if (createdToUtc.HasValue)
                query = query.Where(qe => qe.CreatedOnUtc <= createdToUtc);
            if (loadNotSentItemsOnly)
                query = query.Where(qe => !qe.SentOnUtc.HasValue);
            if (loadOnlyItemsToBeSent)
            {
                var nowUtc = DateTime.UtcNow;
                query = query.Where(qe => !qe.DontSendBeforeDateUtc.HasValue || qe.DontSendBeforeDateUtc.Value <= nowUtc);
            }

            query = query.Where(qe => qe.SentTries < maxSendTries);
            query = loadNewest ?
                //load the newest records
                query.OrderByDescending(qe => qe.CreatedOnUtc) :
                //load by priority
                query.OrderByDescending(qe => qe.PriorityId).ThenBy(qe => qe.CreatedOnUtc);

            var queuedEmails = new PagedList<QueuedEmail>(query, pageIndex, pageSize);
            return queuedEmails;
        }

        /// <summary>
        /// Delete all queued emails
        /// </summary>
        public virtual void DeleteAllEmails()
        {
            //do all databases support "Truncate command"?
            var queuedEmailTableName = _dbContext.GetTableName<QueuedEmail>();
            _dbContext.ExecuteSqlCommand($"TRUNCATE TABLE [{queuedEmailTableName}]");

            //var queuedEmails = _queuedEmailRepository.Table.ToList();
            //foreach (var qe in queuedEmails)
            //    _queuedEmailRepository.Delete(qe);
        }

        #endregion
    }
}