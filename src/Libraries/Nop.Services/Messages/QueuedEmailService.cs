using System;
using System.Collections.Generic;
using System.Linq;

using Nop.Core.Domain.Messages;
using Nop.Data;

namespace Nop.Services.Messages
{
    public partial class QueuedEmailService:IQueuedEmailService
    {
        private readonly IRepository<QueuedEmail> _queuedEmailRepository;
      
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="queuedEmailRepository">Queued email repository</param>
        public QueuedEmailService(IRepository<QueuedEmail> queuedEmailRepository)
        {
            this._queuedEmailRepository = queuedEmailRepository;
        }

        /// <summary>
        /// Inserts a queued email
        /// </summary>
        /// <param name="queuedEmail">Queued email</param>        
        public void InsertQueuedEmail(QueuedEmail queuedEmail)
        {
            if (queuedEmail == null)
                throw new ArgumentNullException("queuedEmail");

            _queuedEmailRepository.Insert(queuedEmail);
        }

        /// <summary>
        /// Updates a queued email
        /// </summary>
        /// <param name="queuedEmail">Queued email</param>
        public void UpdateQueuedEmail(QueuedEmail queuedEmail)
        {
            if (queuedEmail == null)
                throw new ArgumentNullException("queuedEmail");

            _queuedEmailRepository.Update(queuedEmail);
        }

        /// <summary>
        /// Deleted a queued email
        /// </summary>
        /// <param name="queuedEmail">Queued email</param>
        public void DeleteQueuedEmail(QueuedEmail queuedEmail)
        {
            if (queuedEmail == null)
                return;

            _queuedEmailRepository.Delete(queuedEmail);
        }

        /// <summary>
        /// Gets a queued email by identifier
        /// </summary>
        /// <param name="queuedEmailId">Queued email identifier</param>
        /// <returns>Queued email</returns>
        public QueuedEmail GetQueuedEmailById(int queuedEmailId)
        {
            if (queuedEmailId == 0)
                return null;

            var queuedEmail = _queuedEmailRepository.GetById(queuedEmailId);
            return queuedEmail;

        }

        /// <summary>
        /// Gets all queued emails
        /// </summary>
        /// <param name="queuedEmailCount">Email item count. 0 if you want to get all items</param>
        /// <param name="loadNotSentItemsOnly">A value indicating whether to load only not sent emails</param>
        /// <param name="maxSendTries">Maximum send tries</param>
        /// <returns>Queued email list</returns>
        public IList<QueuedEmail> GetAllQueuedEmails(int queuedEmailCount, bool loadNotSentItemsOnly, int maxSendTries)
        {
            return GetAllQueuedEmails(String.Empty, String.Empty, null, null,
                queuedEmailCount, loadNotSentItemsOnly, maxSendTries);
        }

        /// <summary>
        /// Gets all queued emails
        /// </summary>
        /// <param name="fromEmail">From Email</param>
        /// <param name="toEmail">To Email</param>
        /// <param name="startTime">The start time</param>
        /// <param name="endTime">The end time</param>
        /// <param name="queuedEmailCount">Email item count. 0 if you want to get all items</param>
        /// <param name="loadNotSentItemsOnly">A value indicating whether to load only not sent emails</param>
        /// <param name="maxSendTries">Maximum send tries</param>
        /// <returns>Email item list</returns>
        public IList<QueuedEmail> GetAllQueuedEmails(string fromEmail, string toEmail, DateTime? startTime, DateTime? endTime, int queuedEmailCount, bool loadNotSentItemsOnly, int maxSendTries)
        {
            fromEmail = (fromEmail ?? String.Empty).Trim();
            toEmail = (toEmail ?? String.Empty).Trim();

            var query = from qe in _queuedEmailRepository.Table
                        where 
                        (String.IsNullOrEmpty(fromEmail) || qe.From.Contains(fromEmail)) &&
                        (String.IsNullOrEmpty(toEmail) || qe.To.Contains(toEmail)) &&
                        (!startTime.HasValue || qe.CreatedOn >= startTime) &&
                        (!endTime.HasValue || qe.CreatedOn <= endTime) &&
                        (!loadNotSentItemsOnly || qe.SentOn.HasValue) &&
                        qe.SendTries < maxSendTries
                        select qe;

            if (queuedEmailCount > 0)
                query = query.Take(queuedEmailCount);
            query = query.OrderByDescending(qe => qe.Priority).ThenBy(qe => qe.CreatedOn);

            var queuedEmails = query.ToList();
            return queuedEmails;
        }
    }
}
