using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Forums;

namespace Nop.Data.Mapping.Builders.Forums;

/// <summary>
/// Represents a forum group entity builder
/// </summary>
public partial class ForumGroupBuilder : NopEntityBuilder<ForumGroup>
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