﻿using Nop.Services.Logging;
using Nop.Services.ScheduleTasks;

namespace Nop.Services.Messages
{
    /// <summary>
    /// Represents a task for sending queued message 
    /// </summary>
    public partial class QueuedMessagesSendTask : IScheduleTask
    {
        #region Fields

        protected readonly IEmailAccountService _emailAccountService;
        protected readonly IEmailSender _emailSender;
        protected readonly ILogger _logger;
        protected readonly IQueuedEmailService _queuedEmailService;

        #endregion

        #region Ctor

        public QueuedMessagesSendTask(IEmailAccountService emailAccountService,
            IEmailSender emailSender,
            ILogger logger,
            IQueuedEmailService queuedEmailService)
        {
            _emailAccountService = emailAccountService;
            _emailSender = emailSender;
            _logger = logger;
            _queuedEmailService = queuedEmailService;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Executes a task
        /// </summary>
        public virtual async Task ExecuteAsync()
        {
            var maxTries = 3;
            var queuedEmails = await _queuedEmailService.SearchEmailsAsync(null, null, null, null,
                true, true, maxTries, false, 0, 500);
            foreach (var queuedEmail in queuedEmails)
            {
                var bcc = string.IsNullOrWhiteSpace(queuedEmail.Bcc)
                            ? null
                            : queuedEmail.Bcc.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                var cc = string.IsNullOrWhiteSpace(queuedEmail.CC)
                            ? null
                            : queuedEmail.CC.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

                try
                {
                    await _emailSender.SendEmailAsync(await _emailAccountService.GetEmailAccountByIdAsync(queuedEmail.EmailAccountId),
                        queuedEmail.Subject,
                        queuedEmail.Body,
                       queuedEmail.From,
                       queuedEmail.FromName,
                       queuedEmail.To,
                       queuedEmail.ToName,
                       queuedEmail.ReplyTo,
                       queuedEmail.ReplyToName,
                       bcc,
                       cc,
                       queuedEmail.AttachmentFilePath,
                       queuedEmail.AttachmentFileName,
                       queuedEmail.AttachedDownloadId);

                    queuedEmail.SentOnUtc = DateTime.UtcNow;
                }
                catch (Exception exc)
                {
                    await _logger.ErrorAsync($"Error sending e-mail. {exc.Message}", exc);
                }
                finally
                {
                    queuedEmail.SentTries += 1;
                    await _queuedEmailService.UpdateQueuedEmailAsync(queuedEmail);
                }
            }
        }

        #endregion
    }
}