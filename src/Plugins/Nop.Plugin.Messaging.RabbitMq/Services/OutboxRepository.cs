using Nop.Data;
using Nop.Plugin.Messaging.RabbitMq.Domain;

namespace Nop.Plugin.Messaging.RabbitMq.Services;

public class OutboxRepository : IOutboxRepository
{
    private readonly IRepository<OutboxMessage> _repository;

    public OutboxRepository(IRepository<OutboxMessage> repository)
    {
        _repository = repository;
    }

    public async Task InsertAsync(OutboxMessage message)
    {
        await _repository.InsertAsync(message, publishEvent: false);
    }

    public async Task<IList<OutboxMessage>> GetPendingAsync(int batchSize)
    {
        return await _repository.GetAllAsync(q => q
            .Where(m => m.Status == (int)OutboxMessageStatus.Pending)
            .OrderBy(m => m.CreatedAtUtc)
            .Take(batchSize));
    }

    public async Task UpdateAsync(OutboxMessage message)
    {
        await _repository.UpdateAsync(message, publishEvent: false);
    }
}
