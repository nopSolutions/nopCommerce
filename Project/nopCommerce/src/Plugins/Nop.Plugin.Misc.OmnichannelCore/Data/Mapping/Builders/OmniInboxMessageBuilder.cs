using FluentMigrator.Builders.Create.Table;
using Nop.Data.Mapping.Builders;
using Nop.Plugin.Misc.OmnichannelCore.Domains;

namespace Nop.Plugin.Misc.OmnichannelCore.Data.Mapping.Builders;

/// <summary>
/// Represents an inbox message entity builder
/// </summary>
public class OmniInboxMessageBuilder : NopEntityBuilder<OmniInboxMessage>
{
    #region Methods

    /// <summary>
    /// Apply entity configuration
    /// </summary>
    /// <param name="table">Create table expression builder</param>
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
        table
            .WithColumn(nameof(OmniInboxMessage.MessageId)).AsGuid().NotNullable().Indexed()
            .WithColumn(nameof(OmniInboxMessage.EventType)).AsString(200).NotNullable()
            .WithColumn(nameof(OmniInboxMessage.CorrelationId)).AsString(100).Nullable().Indexed()
            .WithColumn(nameof(OmniInboxMessage.Source)).AsString(100).Nullable()
            .WithColumn(nameof(OmniInboxMessage.LastError)).AsString(int.MaxValue).Nullable();
    }

    #endregion
}
