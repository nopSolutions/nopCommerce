using FluentAssertions;
using Nop.Core.Domain.Messages;
using Nop.Services.Messages;
using NUnit.Framework;

namespace Nop.Tests.Nop.Services.Tests.Messages;

[TestFixture]
public class EmailSenderTests : BaseNopTest
{
    private IEmailSender _emailSender;

    [OneTimeSetUp]
    public void SetUp()
    {
        _emailSender = GetService<IEmailSender>();
    }

    [Test]
    public async Task CanSendEmail()
    {
        TestSmtpBuilder.TestSmtpClient.MessageIsSent = false;

        var emailAccount = new EmailAccount
        {
            Id = 1,
            Email = NopTestsDefaults.AdminEmail,
            DisplayName = "Test name",
            Host = "smtp.test.com",
            Port = 25,
            Username = "test_user",
            Password = "test_password",
            EnableSsl = false
        };

        var subject = "Test subject";
        var body = "Test body";
        var fromAddress = NopTestsDefaults.AdminEmail;
        var fromName = "From name";
        var toAddress = "test@test.com";
        var toName = "To name";
        var replyToAddress = NopTestsDefaults.AdminEmail;
        var replyToName = "Reply to name";
        var bcc = new[] { NopTestsDefaults.AdminEmail };
        var cc = new[] { NopTestsDefaults.AdminEmail };

        await _emailSender.SendEmailAsync(emailAccount, subject, body,
            fromAddress, fromName, toAddress, toName,
            replyToAddress, replyToName, bcc, cc);

        TestSmtpBuilder.TestSmtpClient.MessageIsSent.Should().BeTrue();
    }
}