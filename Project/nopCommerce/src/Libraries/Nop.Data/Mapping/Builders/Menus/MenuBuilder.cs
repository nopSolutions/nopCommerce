using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Menus;

namespace Nop.Data.Mapping.Builders.Menus;

/// <summary>
/// Represents a menu entity builder
/// </summary>
public partial class MenuBuilder : NopEntityBuilder<Menu>
{
    #region Methods

    /// <summary>
    /// Apply entity configuration
    /// </summary>
    /// <param name="table">Create table expression builder</param>
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
        table
            .WithColumn(nameof(Menu.Name)).AsString(400).NotNullable()
            .WithColumn(nameof(Menu.CssClass)).AsString(400).Nullable();
    }

    #endregion
}
