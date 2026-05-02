using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Topics;

namespace Nop.Data.Mapping.Builders.Topics;

/// <summary>
/// Represents a topic entity builder
/// </summary>
public partial class TopicBuilder : NopEntityBuilder<Topic>
{
    #region Methods

    /// <summary>
    /// Apply entity configuration
    /// </summary>
    /// <param name="table">Create table expression builder</param>
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
        table.WithColumn(nameof(Topic.SystemName)).AsString(400).Nullable();
    }

    #endregion
}
