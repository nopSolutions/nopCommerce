using System.Diagnostics;
using AwesomeAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Nop.Plugin.Misc.OmnichannelCore;
using Nop.Plugin.Misc.OmnichannelCore.Controllers;
using Nop.Plugin.Misc.OmnichannelCore.Domains;
using Nop.Plugin.Misc.OmnichannelCore.Models.Callbacks;
using Nop.Plugin.Misc.OmnichannelCore.Services;
using NUnit.Framework;

namespace Nop.Tests.Nop.Plugin.Misc.OmnichannelCore.Tests;

[TestFixture]
public class OmnichannelCallbackControllerTests
{
    [Test]
    public async Task PosStockChanged_ReturnsUnauthorized_WhenDemoTokenIsMissing()
    {
        var fixture = CreateFixture(addDemoToken: false);

        var result = await fixture.Controller.PosStockChanged(CreateRequest(sourceVersion: 42, quantityOnHand: 3));

        result.Should().BeOfType<UnauthorizedObjectResult>();
        fixture.InboxRepository.Entities.Should().BeEmpty();
        fixture.StockRepository.Entities.Should().BeEmpty();
    }

    [Test]
    public async Task PosStockChanged_IgnoresDuplicateMessageId()
    {
        var fixture = CreateFixture();
        var request = CreateRequest(sourceVersion: 42, quantityOnHand: 3);

        var firstResult = await fixture.Controller.PosStockChanged(request);
        var duplicateResult = await fixture.Controller.PosStockChanged(request);

        var firstResponse = ReadOkResponse(firstResult);
        var duplicateResponse = ReadOkResponse(duplicateResult);

        firstResponse.Result.Should().Be("applied");
        firstResponse.Duplicate.Should().BeFalse();
        duplicateResponse.Result.Should().Be("duplicate");
        duplicateResponse.Duplicate.Should().BeTrue();

        // The duplicate is rejected via the existing inbox row; no second row is written and the
        // stock projection is untouched.
        fixture.InboxRepository.Entities.Should().ContainSingle();
        fixture.InboxRepository.Entities[0].Status.Should().Be(OmniInboxMessageStatus.Processed);
        fixture.StockRepository.Entities.Should().ContainSingle();
        fixture.StockRepository.Entities[0].QuantityOnHand.Should().Be(3);
    }

    [Test]
    public async Task PosStockChanged_DetectsDuplicateWithinQa2Threshold()
    {
        var fixture = CreateFixture();
        var request = CreateRequest(sourceVersion: 42, quantityOnHand: 3);

        await fixture.Controller.PosStockChanged(request);

        var stopwatch = Stopwatch.StartNew();
        var duplicateResult = await fixture.Controller.PosStockChanged(request);
        stopwatch.Stop();

        var duplicateResponse = ReadOkResponse(duplicateResult);

        duplicateResponse.Duplicate.Should().BeTrue();
        stopwatch.ElapsedMilliseconds.Should().BeLessThanOrEqualTo(50);
    }

    [Test]
    public async Task PosStockChanged_IgnoresStaleSourceVersion()
    {
        var fixture = CreateFixture();

        var currentResult = await fixture.Controller.PosStockChanged(CreateRequest(sourceVersion: 42, quantityOnHand: 5));
        var staleResult = await fixture.Controller.PosStockChanged(CreateRequest(sourceVersion: 41, quantityOnHand: 12));

        var currentResponse = ReadOkResponse(currentResult);
        var staleResponse = ReadOkResponse(staleResult);

        currentResponse.Result.Should().Be("applied");
        staleResponse.Result.Should().Be("stale_ignored");
        staleResponse.Stale.Should().BeTrue();

        // The response echoes the stored (winning) source version, not the rejected stale one.
        staleResponse.SourceVersion.Should().Be(42);
        fixture.StockRepository.Entities.Should().ContainSingle();
        fixture.StockRepository.Entities[0].QuantityOnHand.Should().Be(5);
        fixture.StockRepository.Entities[0].SourceVersion.Should().Be(42);
        fixture.StockRepository.Entities[0].Status.Should().Be(OmniStockSyncStatus.StaleIgnored);
    }

    private static CallbackFixture CreateFixture(bool addDemoToken = true)
    {
        var inboxRepository = new InMemoryRepository<OmniInboxMessage>();
        var stockRepository = new InMemoryRepository<OmniStockSyncState>();
        var controller = new OmnichannelCallbackController(CreateConfiguration(),
            new OmniInboxService(inboxRepository),
            new OmniStockSyncService(stockRepository))
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            }
        };

        if (addDemoToken)
            controller.Request.Headers[OmnichannelCoreDefaults.DemoTokenHeaderName] = OmnichannelCoreDefaults.DefaultDemoToken;

        return new CallbackFixture(controller, inboxRepository, stockRepository);
    }

    private static IConfiguration CreateConfiguration()
    {
        return new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                ["OmnichannelCore:DemoToken"] = OmnichannelCoreDefaults.DefaultDemoToken
            })
            .Build();
    }

    private static PosStockChangedRequest CreateRequest(long sourceVersion, int quantityOnHand)
    {
        return new PosStockChangedRequest
        {
            MessageId = Guid.NewGuid(),
            EventType = OmnichannelCoreDefaults.PosStockChangedEventType,
            OccurredOnUtc = DateTime.UtcNow,
            CorrelationId = Guid.NewGuid().ToString("N"),
            ProductId = 15,
            Sku = "LAPTOP-15",
            WarehouseId = 2,
            QuantityOnHand = quantityOnHand,
            SourceVersion = sourceVersion,
            Source = "pos-sim"
        };
    }

    private static OmnichannelCallbackResponse ReadOkResponse(IActionResult actionResult)
    {
        var okResult = actionResult.Should().BeOfType<OkObjectResult>().Subject;
        return okResult.Value.Should().BeOfType<OmnichannelCallbackResponse>().Subject;
    }

    private sealed record CallbackFixture(OmnichannelCallbackController Controller,
        InMemoryRepository<OmniInboxMessage> InboxRepository,
        InMemoryRepository<OmniStockSyncState> StockRepository);
}
