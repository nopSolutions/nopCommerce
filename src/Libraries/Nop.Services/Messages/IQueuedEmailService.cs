using Nop.Core;
using Nop.Core.Domain.Messages;

namespace Nop.Services.Messages
{
    /// <summary>
    /// Queued email service
    /// </summary>
    public partial interface IQueuedEmailService
    {
        /// <summary>
        /// Inserts a queued email
        /// </summary>
        /// <param name="queuedEmail">Queued email</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task InsertQueuedEmailAsync(QueuedEmail queuedEmail);

        /// <summary>
        /// Updates a queued email
        /// </summary>
        /// <param name="queuedEmail">Queued email</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task UpdateQueuedEmailAsync(QueuedEmail queuedEmail);

        /// <summary>
        /// Deleted a queued email
        /// </summary>
        /// <param name="queuedEmail">Queued email</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task DeleteQueuedEmailAsync(QueuedEmail queuedEmail);

        /// <summary>
        /// Deleted a queued emails
        /// </summary>
        /// <param name="queuedEmails">Queued emails</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task DeleteQueuedEmailsAsync(IList<QueuedEmail> queuedEmails);

        /// <summary>
        /// Gets a queued email by identifier
        /// </summary>
        /// <param name="queuedEmailId">Queued email identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the queued email
        /// </returns>
        Task<QueuedEmail> GetQueuedEmailByIdAsync(int queuedEmailId);

        /// <summary>
        /// Get queued emails by identifiers
        /// </summary>
        /// <param name="queuedEmailIds">queued email identifiers</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the queued emails
        /// </returns>
        Task<IList<QueuedEmail>> GetQueuedEmailsByIdsAsync(int[] queuedEmailIds);

        /// <summary>
        /// Search queued emails
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
        /// The task result contains the queued emails
        /// </returns>
        Task<IPagedList<QueuedEmail>> SearchEmailsAsync(string fromEmail,
            string toEmail, DateTime? createdFromUtc, DateTime? createdToUtc,
            bool loadNotSentItemsOnly, bool loadOnlyItemsToBeSent, int maxSendTries,
            bool loadNewest, int pageIndex = 0, int pageSize = int.MaxValue);

        /// <summary>
        /// Deletes already sent emails
        /// </summary>
        /// <param name="createdFromUtc">Created date from (UTC); null to load all records</param>
        /// <param name="createdToUtc">Created date to (UTC); null to load all records</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the number of deleted emails
        /// </returns>
        Task<int> DeleteAlreadySentEmailsAsync(DateTime? createdFromUtc, DateTime? createdToUtc);

        /// <summary>
        /// Delete all queued emails
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task DeleteAllEmailsAsync();
    }
}
