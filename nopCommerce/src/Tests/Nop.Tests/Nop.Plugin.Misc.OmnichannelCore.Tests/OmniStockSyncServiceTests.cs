using AwesomeAssertions;
using Nop.Plugin.Misc.OmnichannelCore.Domains;
using Nop.Plugin.Misc.OmnichannelCore.Models.Callbacks;
using Nop.Plugin.Misc.OmnichannelCore.Services;
using NUnit.Framework;

namespace Nop.Tests.Nop.Plugin.Misc.OmnichannelCore.Tests;

[TestFixture]
public class OmniStockSyncServiceTests
{
    [Test]
    public async Task ApplyPosStockChangedAsync_AppliesNewerSourceVersion()
    {
        var repository = new InMemoryRepository<OmniStockSyncState>();
        var service = new OmniStockSyncService(repository);

        await service.ApplyPosStockChangedAsync(CreateRequest(quantityOnHand: 3, sourceVersion: 41));
        var result = await service.ApplyPosStockChangedAsync(CreateRequest(quantityOnHand: 4, sourceVersion: 42));

        result.Applied.Should().BeTrue();
        result.Stale.Should().BeFalse();
        repository.Entities.Should().HaveCount(1);
        repository.Entities[0].QuantityOnHand.Should().Be(4);
        repository.Entities[0].SourceVersion.Should().Be(42);
        repository.Entities[0].Status.Should().Be(OmniStockSyncStatus.Current);
    }

    [Test]
    public async Task ApplyPosStockChangedAsync_IgnoresStaleSourceVersion()
    {
        var repository = new InMemoryRepository<OmniStockSyncState>();
        var service = new OmniStockSyncService(repository);

        await service.ApplyPosStockChangedAsync(CreateRequest(quantityOnHand: 5, sourceVersion: 42));
        var result = await service.ApplyPosStockChangedAsync(CreateRequest(quantityOnHand: 12, sourceVersion: 41));

        result.Applied.Should().BeFalse();
        result.Stale.Should().BeTrue();
        repository.Entities.Should().HaveCount(1);
        repository.Entities[0].QuantityOnHand.Should().Be(5);
        repository.Entities[0].SourceVersion.Should().Be(42);
        repository.Entities[0].Status.Should().Be(OmniStockSyncStatus.StaleIgnored);
    }

    private static PosStockChangedRequest CreateRequest(int quantityOnHand, long sourceVersion)
    {
        return new PosStockChangedRequest
        {
            MessageId = Guid.NewGuid(),
            EventType = "pos.stock.changed.v1",
            CorrelationId = "correlation-1",
            ProductId = 15,
            Sku = "LAPTOP-15",
            WarehouseId = 2,
            QuantityOnHand = quantityOnHand,
            SourceVersion = sourceVersion,
            Source = "pos-sim",
            OccurredOnUtc = DateTime.UtcNow
        };
    }
}
