using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using Nop.Core.Domain.Messages;
using Nop.Data;

namespace Nop.Services.Messages
{
    /// <summary>
    /// Queued email service
    /// </summary>
    public partial class QueuedEmailService : IQueuedEmailService
    {
        #region Fields

        private readonly IRepository<QueuedEmail> _queuedEmailRepository;

        #endregion

        #region Ctor

        public QueuedEmailService(IRepository<QueuedEmail> queuedEmailRepository)
        {
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
            _queuedEmailRepository.Insert(queuedEmail);
        }

        /// <summary>
        /// Updates a queued email
        /// </summary>
        /// <param name="queuedEmail">Queued email</param>
        public virtual void UpdateQueuedEmail(QueuedEmail queuedEmail)
        {
            _queuedEmailRepository.Update(queuedEmail);
        }

        /// <summary>
        /// Deleted a queued email
        /// </summary>
        /// <param name="queuedEmail">Queued email</param>
        public virtual void DeleteQueuedEmail(QueuedEmail queuedEmail)
        {
            _queuedEmailRepository.Delete(queuedEmail);
        }

        /// <summary>
        /// Deleted a queued emails
        /// </summary>
        /// <param name="queuedEmails">Queued emails</param>
        public virtual void DeleteQueuedEmails(IList<QueuedEmail> queuedEmails)
        {
            _queuedEmailRepository.Delete(queuedEmails);
        }

        /// <summary>
        /// Gets a queued email by identifier
        /// </summary>
        /// <param name="queuedEmailId">Queued email identifier</param>
        /// <returns>Queued email</returns>
        public virtual QueuedEmail GetQueuedEmailById(int queuedEmailId)
        {
            return _queuedEmailRepository.GetById(queuedEmailId, cache => default);
        }

        /// <summary>
        /// Get queued emails by identifiers
        /// </summary>
        /// <param name="queuedEmailIds">queued email identifiers</param>
        /// <returns>Queued emails</returns>
        public virtual IList<QueuedEmail> GetQueuedEmailsByIds(int[] queuedEmailIds)
        {
            return _queuedEmailRepository.GetByIds(queuedEmailIds);
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
        /// Deletes already sent emails
        /// </summary>
        /// <param name="createdFromUtc">Created date from (UTC); null to load all records</param>
        /// <param name="createdToUtc">Created date to (UTC); null to load all records</param>
        /// <returns>Number of deleted emails</returns>
        public virtual int DeleteAlreadySentEmails(DateTime? createdFromUtc, DateTime? createdToUtc)
        {
            var query = _queuedEmailRepository.Table;

            // only sent emails
            query = query.Where(qe => qe.SentOnUtc.HasValue);

            if (createdFromUtc.HasValue)
                query = query.Where(qe => qe.CreatedOnUtc >= createdFromUtc);
            if (createdToUtc.HasValue)
                query = query.Where(qe => qe.CreatedOnUtc <= createdToUtc);

            var emails = query.ToArray();

            DeleteQueuedEmails(emails);

            return emails.Length;
        }

        /// <summary>
        /// Delete all queued emails
        /// </summary>
        public virtual void DeleteAllEmails()
        {
            _queuedEmailRepository.Truncate();
        }

        #endregion
    }
}