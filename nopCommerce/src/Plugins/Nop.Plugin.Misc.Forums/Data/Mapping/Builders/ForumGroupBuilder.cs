using FluentMigrator.Builders.Create.Table;
using Nop.Data.Mapping.Builders;
using Nop.Plugin.Misc.Forums.Domain;

namespace Nop.Plugin.Misc.Forums.Data.Mapping.Builders;

/// <summary>
/// Represents a forum group entity builder
/// </summary>
public class ForumGroupBuilder : NopEntityBuilder<ForumGroup>
{
    #region Methods

    /// <summary>
    /// Apply entity configuration
    /// </summary>
    /// <param name="table">Create table expression builder</param>
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
        table.WithColumn(nameof(ForumGroup.Name)).AsString(200).NotNullable();
    }

    #endregion
}