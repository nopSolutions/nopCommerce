using System.Text.Json;
using Nop.Core.Domain.Events;
using Nop.Data;
using Nop.Services.Logging;

namespace Nop.Services.Integration;

/// <summary>
/// Spike outbox service implementation
/// </summary>
public partial class SpikeOutboxService : ISpikeOutboxService
{
    #region Fields

    private readonly IRepository<IntegrationEvent> _integrationEventRepository;
    private readonly ILogger _logger;

    #endregion

    #region Ctor

    public SpikeOutboxService(
        IRepository<IntegrationEvent> integrationEventRepository,
        
        ILogger logger)
    {
        _integrationEventRepository = integrationEventRepository;
        _logger = logger;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Writes a spike event to the outbox table on application startup
    /// </summary>
    public async Task WriteStartupEventAsync()
    {
        var eventData = new
        {
            Message = "nopCommerce started successfully",
            Timestamp = DateTime.UtcNow,
            SpikeVersion = "1.0"
        };

        var integrationEvent = new IntegrationEvent
        {
            EventType = "ApplicationStarted",
            EventData = JsonSerializer.Serialize(eventData),
            CreatedOnUtc = DateTime.UtcNow,
            Published = false,
            PublishAttempts = 0
        };

        await _integrationEventRepository.InsertAsync(integrationEvent);

        await _logger.InformationAsync($"Spike: Wrote IntegrationEvent to outbox (Id={integrationEvent.Id})");
    }

    #endregion
}
