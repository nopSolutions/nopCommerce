using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;

using Nop.Core.Domain.Messages;

namespace Nop.Services.Messages
{
    public partial class EmailSender:IEmailSender
    {
        /// <summary>
        /// Sends an email
        /// </summary>
        /// <param name="subject">Subject</param>
        /// <param name="body">Body</param>
        /// <param name="from">From</param>
        /// <param name="to">To</param>
        /// <param name="emailAccount">Email account to use</param>
        public void SendEmail(string subject, string body, string from, string to,
            EmailAccount emailAccount)
        {
            SendEmail(subject, body, new MailAddress(from), new MailAddress(to),
                new List<String>(), new List<String>(), emailAccount);
        }

        /// <summary>
        /// Sends an email
        /// </summary>
        /// <param name="subject">Subject</param>
        /// <param name="body">Body</param>
        /// <param name="from">From</param>
        /// <param name="to">To</param>
        /// <param name="emailAccount">Email account to use</param>
        public void SendEmail(string subject, string body, MailAddress from,
            MailAddress to, EmailAccount emailAccount)
        {
            SendEmail(subject, body, from, to, new List<String>(), new List<String>(), emailAccount);
        }

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
        public void SendEmail(string subject, string body,
            MailAddress from, MailAddress to, List<string> bcc,
            List<string> cc, EmailAccount emailAccount)
        {
            var message = new MailMessage();
            message.From = from;
            message.To.Add(to);
            if (null != bcc)
                foreach (string address in bcc)
                {
                    if (address != null)
                    {
                        if (!String.IsNullOrEmpty(address.Trim()))
                        {
                            message.Bcc.Add(address.Trim());
                        }
                    }
                }
            if (null != cc)
                foreach (string address in cc)
                {
                    if (address != null)
                    {
                        if (!String.IsNullOrEmpty(address.Trim()))
                        {
                            message.CC.Add(address.Trim());
                        }
                    }
                }
            message.Subject = subject;
            message.Body = body;
            message.IsBodyHtml = true;

            var smtpClient = new SmtpClient();
            smtpClient.UseDefaultCredentials = emailAccount.UseDefaultCredentials;
            smtpClient.Host = emailAccount.Host;
            smtpClient.Port = emailAccount.Port;
            smtpClient.EnableSsl = emailAccount.EnableSSL;
            if (emailAccount.UseDefaultCredentials)
                smtpClient.Credentials = CredentialCache.DefaultNetworkCredentials;
            else
                smtpClient.Credentials = new NetworkCredential(emailAccount.Username, emailAccount.Password);
            smtpClient.Send(message);
        }
    }
}
