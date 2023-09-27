using MailKit;
using MailKit.Net.Smtp;
using MimeKit;
using Nop.Core.Domain.Messages;
using Nop.Services.Messages;

namespace Nop.Tests
{
    public class TestSmtpBuilder : SmtpBuilder
    {
        public TestSmtpBuilder(EmailAccountSettings emailAccountSettings, IEmailAccountService emailAccountService) : base(emailAccountSettings, emailAccountService)
        {
        }

        public override Task<SmtpClient> BuildAsync(EmailAccount emailAccount = null)
        {
            return Task.FromResult<SmtpClient>(new TestSmtpClient());
        }

        public class TestSmtpClient : SmtpClient
        {
            public override Task<string> SendAsync(MimeMessage message,
                CancellationToken cancellationToken = default,
                ITransferProgress progress = null)
            {
                MessageIsSent = true;
                return Task.FromResult(string.Empty);
            }

            public static bool MessageIsSent { get; set; }
        }
    }
}
