using System.Text.Json;
using Nop.Core.Domain.Events;
using Nop.Data;
using Nop.Services.Logging;

namespace Nop.Services.Integration;

public partial class OutboxService : IOutboxService
{
    private readonly IRepository<IntegrationEvent> _integrationEventRepository;
    private readonly ILogger _logger;

    public OutboxService(
        IRepository<IntegrationEvent> integrationEventRepository,
        ILogger logger)
    {
        _integrationEventRepository = integrationEventRepository;
        _logger = logger;
    }

    public async Task WriteEventAsync(string eventType, object data)
    {
        var integrationEvent = new IntegrationEvent
        {
            EventType = eventType,
            EventData = JsonSerializer.Serialize(data),
            CreatedOnUtc = DateTime.UtcNow,
            Published = false,
            PublishAttempts = 0
        };

        await _integrationEventRepository.InsertAsync(integrationEvent);

        await _logger.InformationAsync($"Outbox: wrote {eventType} (Id={integrationEvent.Id})");
    }
}
