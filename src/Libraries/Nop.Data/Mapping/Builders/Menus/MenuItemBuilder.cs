using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Menus;
using Nop.Data.Extensions;

namespace Nop.Data.Mapping.Builders.Menus;

/// <summary>
/// Represents a menu item entity builder
/// </summary>
public partial class MenuItemBuilder : NopEntityBuilder<MenuItem>
{
    #region Methods

    /// <summary>
    /// Apply entity configuration
    /// </summary>
    /// <param name="table">Create table expression builder</param>
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
        table
            .WithColumn(nameof(MenuItem.Title)).AsString(400).Nullable()
            .WithColumn(nameof(MenuItem.CssClass)).AsString(400).Nullable()
            .WithColumn(nameof(MenuItem.RouteName)).AsString(400).Nullable()
            .WithColumn(nameof(MenuItem.MenuId)).AsInt32().ForeignKey<Menu>();
    }

    #endregion
}
