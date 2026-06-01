using System.Text;
using Nop.Plugin.Messaging.RabbitMq.Domain;
using Nop.Services.Logging;
using Nop.Services.ScheduleTasks;
using RabbitMQ.Client;

namespace Nop.Plugin.Messaging.RabbitMq.Services;

public class OutboxDispatcherTask : IScheduleTask
{
    private readonly IOutboxRepository _outboxRepository;
    private readonly IRabbitMqConnectionFactory _connectionFactory;
    private readonly RabbitMqSettings _settings;
    private readonly ILogger _logger;

    public OutboxDispatcherTask(
        IOutboxRepository outboxRepository,
        IRabbitMqConnectionFactory connectionFactory,
        RabbitMqSettings settings,
        ILogger logger)
    {
        _outboxRepository = outboxRepository;
        _connectionFactory = connectionFactory;
        _settings = settings;
        _logger = logger;
    }

    public async Task ExecuteAsync()
    {
        var pending = await _outboxRepository.GetPendingAsync(_settings.DispatcherBatchSize);
        if (pending.Count == 0)
            return;

        IChannel channel;
        try
        {
            channel = await _connectionFactory.CreateChannelAsync();
        }
        catch (Exception ex)
        {
            await _logger.ErrorAsync($"[OutboxDispatcher] Cannot connect to RabbitMQ: {ex.Message}", ex);
            return;
        }

        await using (channel)
        {
            foreach (var message in pending)
            {
                try
                {
                    var body = new ReadOnlyMemory<byte>(Encoding.UTF8.GetBytes(message.Payload));
                    var properties = new BasicProperties
                    {
                        Persistent = true,
                        ContentType = "application/json",
                        MessageId = message.AggregateId
                    };

                    await channel.BasicPublishAsync(
                        exchange: message.Exchange ?? _settings.ExchangeName,
                        routingKey: message.EventType,
                        mandatory: false,
                        basicProperties: properties,
                        body: body);

                    message.Status = (int)OutboxMessageStatus.Sent;
                    message.SentAtUtc = DateTime.UtcNow;
                    await _outboxRepository.UpdateAsync(message);

                    await _logger.InformationAsync($"[OutboxDispatcher] Sent {message.EventType} for AggregateId={message.AggregateId}");
                }
                catch (Exception ex)
                {
                    message.AttemptCount++;
                    if (message.AttemptCount >= _settings.DispatcherMaxAttempts)
                    {
                        message.Status = (int)OutboxMessageStatus.Failed;
                        await _logger.ErrorAsync($"[OutboxDispatcher] Giving up on {message.AggregateId} after {message.AttemptCount} attempts: {ex.Message}", ex);
                    }

                    await _outboxRepository.UpdateAsync(message);
                }
            }
        }
    }
}
