using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Topics;

namespace Nop.Data.Mapping.Builders.Topics;

/// <summary>
/// Represents a topic template entity builder
/// </summary>
public partial class TopicTemplateBuilder : NopEntityBuilder<TopicTemplate>
{
    #region Methods

    /// <summary>
    /// Apply entity configuration
    /// </summary>
    /// <param name="table">Create table expression builder</param>
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
        table
            .WithColumn(nameof(TopicTemplate.Name)).AsString(400).NotNullable()
            .WithColumn(nameof(TopicTemplate.ViewPath)).AsString(400).NotNullable();
    }

    #endregion
}