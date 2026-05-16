using FluentMigrator.Builders.Create.Table;
using Nop.Data.Mapping.Builders;
using Nop.Plugin.Messaging.RabbitMq.Domain;

namespace Nop.Plugin.Messaging.RabbitMq.Data.Mapping;

public class OutboxMessageBuilder : NopEntityBuilder<OutboxMessage>
{
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
        table
            .WithColumn(nameof(OutboxMessage.AggregateId)).AsString(200).NotNullable()
            .WithColumn(nameof(OutboxMessage.EventType)).AsString(200).NotNullable()
            .WithColumn(nameof(OutboxMessage.Payload)).AsString(int.MaxValue).NotNullable()
            .WithColumn(nameof(OutboxMessage.SentAtUtc)).AsDateTime().Nullable();
    }
}
