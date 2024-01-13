using FluentAssertions;
using Nop.Core.Domain.Messages;
using Nop.Data;
using Nop.Services.Messages;
using NUnit.Framework;

namespace Nop.Tests.Nop.Services.Tests.Messages;

[TestFixture]
public class QueuedEmailServiceTests : BaseNopTest
{
    private IQueuedEmailService _queuedEmailService;
    private IRepository<QueuedEmail> _queuedEmailRepository;
    private List<QueuedEmail> _emails;
    private string _testEmail;

    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {
        _queuedEmailService = GetService<IQueuedEmailService>();
        _queuedEmailRepository = GetService<IRepository<QueuedEmail>>();

        await _queuedEmailRepository.TruncateAsync();
        _testEmail = "test@test.com";
    }

    [TearDown]
    public async Task TearDown()
    {
        await _queuedEmailRepository.TruncateAsync();
    }

    [SetUp]
    public async Task SetUp()
    {
        _emails =
        [
            new() {From = NopTestsDefaults.AdminEmail, To = _testEmail, EmailAccountId = 1, SentTries = 5},
        new() {From = NopTestsDefaults.AdminEmail, To = _testEmail, EmailAccountId = 1},
        new() {From = NopTestsDefaults.AdminEmail, To = _testEmail, EmailAccountId = 1},
        new() {From = NopTestsDefaults.AdminEmail, To = _testEmail, EmailAccountId = 1, SentOnUtc = DateTime.UtcNow},
        new() {From = NopTestsDefaults.AdminEmail, To = _testEmail, EmailAccountId = 1, SentOnUtc = DateTime.UtcNow}
            ];

        foreach (var queuedEmail in _emails)
            await _queuedEmailService.InsertQueuedEmailAsync(queuedEmail);
    }

    [Test]
    public async Task CanCRUD()
    {
        var queuedEmails = await _queuedEmailRepository.GetAllAsync(query => query);
        queuedEmails.Count.Should().Be(_emails.Count);

        var email = await _queuedEmailService.GetQueuedEmailByIdAsync(_emails[0].Id);
        email.Body = "test";

        await _queuedEmailService.UpdateQueuedEmailAsync(email);
        (await _queuedEmailRepository.GetByIdAsync(_emails[0].Id)).Body.Should().Be(email.Body);

        await _queuedEmailService.DeleteQueuedEmailAsync(email);

        queuedEmails = await _queuedEmailRepository.GetAllAsync(query => query);
        queuedEmails.Count.Should().Be(_emails.Count - 1);

        await _queuedEmailService.DeleteQueuedEmailsAsync(_emails.Take(3).ToList());

        queuedEmails = await _queuedEmailRepository.GetAllAsync(query => query);
        queuedEmails.Count.Should().Be(_emails.Count - 3);

        await _queuedEmailService.DeleteAllEmailsAsync();

        queuedEmails = await _queuedEmailRepository.GetAllAsync(query => query);
        queuedEmails.Count.Should().Be(0);
    }


    [Test]
    public async Task CanGetQueuedEmailsByIds()
    {
        var queuedEmails =
            await _queuedEmailService.GetQueuedEmailsByIdsAsync(_emails.Take(3).Select(e => e.Id).ToArray());
        queuedEmails.Count.Should().Be(3);
    }

    [Test]
    public async Task CanSearchEmails()
    {
        var loadNotSentItemsOnly = true;
        var loadOnlyItemsToBeSent = true;
        var maxSendTries = int.MaxValue;
        var loadNewest = false;

        async Task<int> getCountAsync()
        {
            var emails = await _queuedEmailService.SearchEmailsAsync(NopTestsDefaults.AdminEmail, _testEmail, null,
                null, loadNotSentItemsOnly, loadOnlyItemsToBeSent, maxSendTries, loadNewest);

            return emails.Count;
        }

        (await getCountAsync()).Should().Be(3);
        loadNotSentItemsOnly = false;
        (await getCountAsync()).Should().Be(5);
        loadOnlyItemsToBeSent = false;
        (await getCountAsync()).Should().Be(5);
        loadNotSentItemsOnly = true;
        (await getCountAsync()).Should().Be(3);
        loadNotSentItemsOnly = false;
        maxSendTries = 1;
        (await getCountAsync()).Should().Be(4);
    }

    [Test]
    public async Task CanDeleteAlreadySentEmails()
    {
        await _queuedEmailService.DeleteAlreadySentEmailsAsync(null, null);
        var queuedEmails = await _queuedEmailRepository.GetAllAsync(query => query);
        queuedEmails.Count.Should().Be(_emails.Count(e => !e.SentOnUtc.HasValue));
    }
}