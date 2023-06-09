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

        protected readonly IRepository<QueuedEmail> _queuedEmailRepository;

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
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task InsertQueuedEmailAsync(QueuedEmail queuedEmail)
        {
            await _queuedEmailRepository.InsertAsync(queuedEmail);
        }

        /// <summary>
        /// Updates a queued email
        /// </summary>
        /// <param name="queuedEmail">Queued email</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task UpdateQueuedEmailAsync(QueuedEmail queuedEmail)
        {
            await _queuedEmailRepository.UpdateAsync(queuedEmail);
        }

        /// <summary>
        /// Deleted a queued email
        /// </summary>
        /// <param name="queuedEmail">Queued email</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeleteQueuedEmailAsync(QueuedEmail queuedEmail)
        {
            await _queuedEmailRepository.DeleteAsync(queuedEmail);
        }

        /// <summary>
        /// Deleted a queued emails
        /// </summary>
        /// <param name="queuedEmails">Queued emails</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeleteQueuedEmailsAsync(IList<QueuedEmail> queuedEmails)
        {
            await _queuedEmailRepository.DeleteAsync(queuedEmails);
        }

        /// <summary>
        /// Gets a queued email by identifier
        /// </summary>
        /// <param name="queuedEmailId">Queued email identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the queued email
        /// </returns>
        public virtual async Task<QueuedEmail> GetQueuedEmailByIdAsync(int queuedEmailId)
        {
            return await _queuedEmailRepository.GetByIdAsync(queuedEmailId, cache => default);
        }

        /// <summary>
        /// Get queued emails by identifiers
        /// </summary>
        /// <param name="queuedEmailIds">queued email identifiers</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the queued emails
        /// </returns>
        public virtual async Task<IList<QueuedEmail>> GetQueuedEmailsByIdsAsync(int[] queuedEmailIds)
        {
            return await _queuedEmailRepository.GetByIdsAsync(queuedEmailIds);
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
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the email item list
        /// </returns>
        public virtual async Task<IPagedList<QueuedEmail>> SearchEmailsAsync(string fromEmail,
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

            var queuedEmails = await query.ToPagedListAsync(pageIndex, pageSize);

            return queuedEmails;
        }

        /// <summary>
        /// Deletes already sent emails
        /// </summary>
        /// <param name="createdFromUtc">Created date from (UTC); null to load all records</param>
        /// <param name="createdToUtc">Created date to (UTC); null to load all records</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the number of deleted emails
        /// </returns>
        public virtual async Task<int> DeleteAlreadySentEmailsAsync(DateTime? createdFromUtc, DateTime? createdToUtc)
        {
            var query = _queuedEmailRepository.Table;

            // only sent emails
            query = query.Where(qe => qe.SentOnUtc.HasValue);

            if (createdFromUtc.HasValue)
                query = query.Where(qe => qe.CreatedOnUtc >= createdFromUtc);
            if (createdToUtc.HasValue)
                query = query.Where(qe => qe.CreatedOnUtc <= createdToUtc);

            var emails = query.ToArray();

            await DeleteQueuedEmailsAsync(emails);

            return emails.Length;
        }

        /// <summary>
        /// Delete all queued emails
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeleteAllEmailsAsync()
        {
            await _queuedEmailRepository.TruncateAsync();
        }

        #endregion
    }
}