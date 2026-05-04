using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Events;

namespace Nop.Data.Mapping.Builders.Events;

/// <summary>
/// Represents an integration event entity builder
/// </summary>
public partial class IntegrationEventBuilder : NopEntityBuilder<IntegrationEvent>
{
    #region Methods

    /// <summary>
    /// Apply entity configuration
    /// </summary>
    /// <param name="table">Create table expression builder</param>
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
        table.WithColumn(nameof(IntegrationEvent.EventType))
            .AsString(200)
            .NotNullable();

        table.WithColumn(nameof(IntegrationEvent.EventData))
            .AsString(int.MaxValue)
            .NotNullable();

        table.WithColumn(nameof(IntegrationEvent.LastError))
            .AsString(4000)
            .Nullable();
    }

    #endregion
}
