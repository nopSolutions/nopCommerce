using FluentMigrator.Builders.Create.Table;
using Nop.Data.Mapping.Builders;
using Nop.Plugin.Misc.OmnichannelCore.Domains;

namespace Nop.Plugin.Misc.OmnichannelCore.Data.Mapping.Builders;

/// <summary>
/// Represents an outbox message entity builder
/// </summary>
public class OmniOutboxMessageBuilder : NopEntityBuilder<OmniOutboxMessage>
{
    #region Methods

    /// <summary>
    /// Apply entity configuration
    /// </summary>
    /// <param name="table">Create table expression builder</param>
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
        table
            .WithColumn(nameof(OmniOutboxMessage.MessageId)).AsGuid().NotNullable().Indexed()
            .WithColumn(nameof(OmniOutboxMessage.EventType)).AsString(200).NotNullable()
            .WithColumn(nameof(OmniOutboxMessage.CorrelationId)).AsString(100).Nullable().Indexed()
            .WithColumn(nameof(OmniOutboxMessage.OrderGuid)).AsGuid().Nullable().Indexed()
            .WithColumn(nameof(OmniOutboxMessage.Payload)).AsString(int.MaxValue).NotNullable()
            .WithColumn(nameof(OmniOutboxMessage.StatusId)).AsInt32().NotNullable().Indexed()
            .WithColumn(nameof(OmniOutboxMessage.LastError)).AsString(int.MaxValue).Nullable();
    }

    #endregion
}
