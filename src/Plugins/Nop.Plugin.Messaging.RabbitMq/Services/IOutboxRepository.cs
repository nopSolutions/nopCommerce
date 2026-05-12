using Nop.Plugin.Messaging.RabbitMq.Domain;

namespace Nop.Plugin.Messaging.RabbitMq.Services;

public interface IOutboxRepository
{
    Task InsertAsync(OutboxMessage message);
    Task<IList<OutboxMessage>> GetPendingAsync(int batchSize);
    Task UpdateAsync(OutboxMessage message);
}
