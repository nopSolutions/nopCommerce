using System;
using System.Net;
using MailKit.Net.Smtp;
using MailKit.Security;
using Nop.Core;
using Nop.Core.Domain.Messages;

namespace Nop.Services.Messages
{
    /// <summary>
    /// SMTP Builder
    /// </summary>
    public class SmtpBuilder : ISmtpBuilder
    {
        #region Fields

        private readonly EmailAccountSettings _emailAccountSettings;
        private readonly IEmailAccountService _emailAccountService;

        #endregion

        #region Ctor

        public SmtpBuilder(EmailAccountSettings emailAccountSettings, IEmailAccountService emailAccountService)
        {
            _emailAccountSettings = emailAccountSettings;
            _emailAccountService = emailAccountService;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Create a new SMTP client for a specific email account
        /// </summary>
        /// <param name="emailAccount">Email account to use. If null, then would be used EmailAccount by default</param>
        /// <returns>An SMTP client that can be used to send email messages</returns>
        public virtual SmtpClient Build(EmailAccount emailAccount = null)
        {
            if (emailAccount is null)
            {
                emailAccount = _emailAccountService.GetEmailAccountById(_emailAccountSettings.DefaultEmailAccountId)
                ?? throw new NopException("Email account could not be loaded");
            }

            var client = new SmtpClient();

            try
            {
                client.Connect(
                     emailAccount.Host,
                     emailAccount.Port,
                     emailAccount.EnableSsl ? SecureSocketOptions.SslOnConnect : SecureSocketOptions.StartTlsWhenAvailable
                 );

                client.Authenticate(emailAccount.UseDefaultCredentials ?
                        CredentialCache.DefaultNetworkCredentials :
                        new NetworkCredential(emailAccount.Username, emailAccount.Password));

                return client;
            }
            catch (Exception ex)
            {
                client.Dispose();
                throw new NopException(ex.Message, ex);
            }
        }

        #endregion
    }
}