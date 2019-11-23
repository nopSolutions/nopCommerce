using MailKit.Net.Smtp;
using Nop.Core.Domain.Messages;

namespace Nop.Services.Messages
{
    /// <summary>
    /// SMTP Builder
    /// </summary>
    public interface ISmtpBuilder
    {
        /// <summary>
        /// Create a new SMTP client for a specific email account
        /// </summary>
        /// <param name="emailAccount">Email account to use. If null, then would be used EmailAccount by default</param>
        /// <returns>An SMTP client that can be used to send email messages</returns>
        SmtpClient Build(EmailAccount emailAccount = null);
    }
}
