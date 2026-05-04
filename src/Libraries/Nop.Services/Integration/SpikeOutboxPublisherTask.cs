using Nop.Core.Domain.Events;
using Nop.Data;
using Nop.Services.Integration.RabbitMQ;
using Nop.Services.Logging;
using Nop.Services.ScheduleTasks;

namespace Nop.Services.Integration;

/// <summary>
/// Spike task that publishes unpublished integration events from the outbox to RabbitMQ
/// </summary>
public partial class SpikeOutboxPublisherTask : IScheduleTask
{
    #region Fields

    private readonly IRepository<IntegrationEvent> _integrationEventRepository;
    private readonly IRabbitMqPublisher _rabbitMqPublisher;
    private readonly ILogger _logger;

    #endregion

    #region Ctor

    public SpikeOutboxPublisherTask(
        IRepository<IntegrationEvent> integrationEventRepository,
        IRabbitMqPublisher rabbitMqPublisher,
        ILogger logger)
    {
        _integrationEventRepository = integrationEventRepository;
        _rabbitMqPublisher = rabbitMqPublisher;
        _logger = logger;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Executes the outbox publisher task
    /// </summary>
    public async Task ExecuteAsync()
    {
        await _logger.InformationAsync("Spike: Outbox publisher task started");

        var unpublishedEvents = await _integrationEventRepository.GetAllAsync(
            query => query.Where(e => !e.Published)
        );

        await _logger.InformationAsync($"Spike: Found {unpublishedEvents.Count} unpublished events");

        foreach (var integrationEvent in unpublishedEvents)
        {
            try
            {
                await _rabbitMqPublisher.PublishAsync(
                    exchange: "verdemart.events",
                    routingKey: integrationEvent.EventType,
                    messageBody: integrationEvent.EventData
                );

                integrationEvent.Published = true;
                integrationEvent.PublishedOnUtc = DateTime.UtcNow;
                integrationEvent.PublishAttempts += 1;
                integrationEvent.LastError = null;

                await _integrationEventRepository.UpdateAsync(integrationEvent);

                await _logger.InformationAsync($"Spike: Published IntegrationEvent Id={integrationEvent.Id}");
            }
            catch (Exception ex)
            {
                integrationEvent.PublishAttempts += 1;
                integrationEvent.LastError = ex.Message;

                await _integrationEventRepository.UpdateAsync(integrationEvent);

                await _logger.ErrorAsync($"Spike: Failed to publish IntegrationEvent Id={integrationEvent.Id}: {ex.Message}", ex);
            }
        }

        await _logger.InformationAsync("Spike: Outbox publisher task completed");
    }

    #endregion
}
