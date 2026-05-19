using Nop.Core.Configuration;
using Nop.Core.Domain.Events;
using Nop.Data;
using Nop.Services.Integration.RabbitMQ;
using Nop.Services.Logging;
using Nop.Services.ScheduleTasks;

namespace Nop.Services.Integration;

/// <summary>
/// Polls the IntegrationEvent outbox and publishes unpublished events to RabbitMQ
/// </summary>
public partial class OutboxPublisherTask : IScheduleTask
{
    private readonly IRepository<IntegrationEvent> _integrationEventRepository;
    private readonly IRabbitMqPublisher _rabbitMqPublisher;
    private readonly IntegrationConfig _integrationConfig;
    private readonly ILogger _logger;

    public OutboxPublisherTask(
        IRepository<IntegrationEvent> integrationEventRepository,
        IRabbitMqPublisher rabbitMqPublisher,
        IntegrationConfig integrationConfig,
        ILogger logger)
    {
        _integrationEventRepository = integrationEventRepository;
        _rabbitMqPublisher = rabbitMqPublisher;
        _integrationConfig = integrationConfig;
        _logger = logger;
    }

    public async Task ExecuteAsync()
    {
        var unpublished = await _integrationEventRepository.GetAllAsync(
            query => query.Where(e => !e.Published).OrderBy(e => e.Id));

        if (unpublished.Count == 0)
            return;

        foreach (var integrationEvent in unpublished)
        {
            try
            {
                await _rabbitMqPublisher.PublishAsync(
                    exchange: _integrationConfig.EventsExchange,
                    routingKey: integrationEvent.EventType,
                    messageBody: integrationEvent.EventData);

                integrationEvent.Published = true;
                integrationEvent.PublishedOnUtc = DateTime.UtcNow;
                integrationEvent.PublishAttempts += 1;
                integrationEvent.LastError = null;

                await _integrationEventRepository.UpdateAsync(integrationEvent);
            }
            catch (Exception ex)
            {
                integrationEvent.PublishAttempts += 1;
                integrationEvent.LastError = ex.Message;

                await _integrationEventRepository.UpdateAsync(integrationEvent);

                await _logger.ErrorAsync(
                    $"Outbox: failed to publish IntegrationEvent Id={integrationEvent.Id}: {ex.Message}", ex);
            }
        }
    }
}
