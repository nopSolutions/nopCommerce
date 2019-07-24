using System.Collections.Generic;
using Nop.Core;
using Nop.Core.Domain.Messages;
using Nop.Core.Infrastructure;
using Nop.Services.Media;
using Nop.Services.Messages;

namespace Nop.Plugin.Misc.SendinBlue.Services
{
    /// <summary>
    /// Represents overridden email sender
    /// </summary>
    public class SendinBlueEmailSender : EmailSender
    {
        #region Fields

        private readonly IStoreContext _storeContext;
        private readonly SendinBlueSettings _sendinBlueSettings;

        #endregion

        #region Ctor

        public SendinBlueEmailSender(IDownloadService downloadService,
            INopFileProvider fileProvider,
            IStoreContext storeContext,
            SendinBlueSettings sendinBlueSettings) : base(downloadService, fileProvider)
        {
            _storeContext = storeContext;
            _sendinBlueSettings = sendinBlueSettings;
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
        public override void SendEmail(EmailAccount emailAccount, string subject, string body,
            string fromAddress, string fromName, string toAddress, string toName,
            string replyTo = null, string replyToName = null,
            IEnumerable<string> bcc = null, IEnumerable<string> cc = null,
            string attachmentFilePath = null, string attachmentFileName = null,
            int attachedDownloadId = 0, IDictionary<string, string> headers = null)
        {
            //add store identifier in email headers
            if (emailAccount.Id == _sendinBlueSettings.EmailAccountId)
            {
                headers = headers ?? new Dictionary<string, string>();
                headers.Add(SendinBlueDefaults.EmailCustomHeader, _storeContext.CurrentStore.Id.ToString());
            }

            base.SendEmail(emailAccount, subject, body, fromAddress, fromName, toAddress, toName, replyTo, replyToName, bcc, cc, attachmentFilePath, attachmentFileName, attachedDownloadId, headers);
        }

        #endregion
    }
}