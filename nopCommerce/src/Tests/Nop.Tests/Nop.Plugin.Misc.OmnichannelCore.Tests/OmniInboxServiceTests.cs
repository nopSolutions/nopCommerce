using AwesomeAssertions;
using Nop.Plugin.Misc.OmnichannelCore.Domains;
using Nop.Plugin.Misc.OmnichannelCore.Models.Callbacks;
using Nop.Plugin.Misc.OmnichannelCore.Services;
using NUnit.Framework;

namespace Nop.Tests.Nop.Plugin.Misc.OmnichannelCore.Tests;

[TestFixture]
public class OmniInboxServiceTests
{
    [Test]
    public async Task TryBeginProcessingAsync_DetectsDuplicateMessageId()
    {
        var repository = new InMemoryRepository<OmniInboxMessage>();
        var service = new OmniInboxService(repository);
        var messageId = Guid.NewGuid();
        var context = new InboxMessageContext
        {
            MessageId = messageId,
            EventType = "pos.stock.changed.v1",
            CorrelationId = "correlation-1",
            Source = "pos-sim"
        };

        var firstResult = await service.TryBeginProcessingAsync(context);
        await service.MarkProcessedAsync(firstResult.InboxMessage);
        var duplicateResult = await service.TryBeginProcessingAsync(context);

        firstResult.IsDuplicate.Should().BeFalse();
        duplicateResult.IsDuplicate.Should().BeTrue();

        // The dedup check returns the already-stored row rather than inserting a second one,
        // so the inbox keeps a single message keyed by messageId.
        repository.Entities.Should().ContainSingle();
        repository.Entities[0].MessageId.Should().Be(messageId);
        repository.Entities[0].Status.Should().Be(OmniInboxMessageStatus.Processed);
        duplicateResult.InboxMessage.MessageId.Should().Be(messageId);
        duplicateResult.InboxMessage.Status.Should().Be(OmniInboxMessageStatus.Processed);
    }
}
