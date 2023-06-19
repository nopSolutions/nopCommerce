using Nop.Core;
using Nop.Core.Domain.Messages;
using Nop.Core.Infrastructure;
using Nop.Services.Media;
using Nop.Services.Messages;

namespace Nop.Plugin.Misc.Brevo.Services
{
    /// <summary>
    /// Represents overridden email sender
    /// </summary>
    public class BrevoEmailSender : EmailSender
    {
        #region Fields

        protected readonly BrevoSettings _brevoSettings;
        protected readonly IStoreContext _storeContext;

        #endregion

        #region Ctor

        public BrevoEmailSender(BrevoSettings brevoSettings,
            IDownloadService downloadService,
            INopFileProvider fileProvider,
            ISmtpBuilder smtpBuilder,
            IStoreContext storeContext
            ) : base(downloadService, fileProvider, smtpBuilder)
        {
            _brevoSettings = brevoSettings;
            _storeContext = storeContext;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Sends an email
        /// </summary>
        /// <param name="emailAccount">Email account to use</param>
        /// <param name="subject">Subject</param>
        /// <param name="body">Body</param>
        /// <param name="fromAddress">From address</param>
        /// <param name="fromName">From display name</param>
        /// <param name="toAddress">To address</param>
        /// <param name="toName">To display name</param>
        /// <param name="replyTo">ReplyTo address</param>
        /// <param name="replyToName">ReplyTo display name</param>
        /// <param name="bcc">BCC addresses list</param>
        /// <param name="cc">CC addresses list</param>
        /// <param name="attachmentFilePath">Attachment file path</param>
        /// <param name="attachmentFileName">Attachment file name. If specified, then this file name will be sent to a recipient. Otherwise, "AttachmentFilePath" name will be used.</param>
        /// <param name="attachedDownloadId">Attachment download ID (another attachedment)</param>
        /// <param name="headers">Headers</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public override async Task SendEmailAsync(EmailAccount emailAccount, string subject, string body,
            string fromAddress, string fromName, string toAddress, string toName,
            string replyTo = null, string replyToName = null,
            IEnumerable<string> bcc = null, IEnumerable<string> cc = null,
            string attachmentFilePath = null, string attachmentFileName = null,
            int attachedDownloadId = 0, IDictionary<string, string> headers = null)
        {
            //add store identifier in email headers
            if (emailAccount.Id == _brevoSettings.EmailAccountId)
            {
                var store = await _storeContext.GetCurrentStoreAsync();
                headers ??= new Dictionary<string, string>();
                headers.Add(BrevoDefaults.EmailCustomHeader, store.Id.ToString());
            }

            await base.SendEmailAsync(emailAccount, subject, body, fromAddress, fromName, toAddress, toName, replyTo, replyToName, bcc, cc, attachmentFilePath, attachmentFileName, attachedDownloadId, headers);
        }

        #endregion
    }
}