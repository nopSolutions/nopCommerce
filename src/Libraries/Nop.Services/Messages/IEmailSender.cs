using System.Collections.Generic;
using System.Net.Mail;

using Nop.Core.Domain.Messages;

namespace Nop.Services.Messages
{
    public partial interface IEmailSender
    {
        /// <summary>
        /// Sends an email
        /// </summary>
        /// <param name="subject">Subject</param>
        /// <param name="body">Body</param>
        /// <param name="from">From</param>
        /// <param name="to">To</param>
        /// <param name="bcc">BCC</param>
        /// <param name="cc">CC</param>
        /// <param name="emailAccount">Email account to use</param>
        void SendEmail(EmailAccount emailAccount, string subject, string body,
            string fromAddress, string fromName, string toAddress, string toName,
            IEnumerable<string> bcc = null,
            IEnumerable<string> cc = null);
    }
}
