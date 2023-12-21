using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Catalog;

namespace Nop.Data.Mapping.Builders.Catalog;

/// <summary>
/// Represents a product template entity builder
/// </summary>
public partial class ProductTemplateBuilder : NopEntityBuilder<ProductTemplate>
{
    #region Methods

    /// <summary>
    /// Apply entity configuration
    /// </summary>
    /// <param name="table">Create table expression builder</param>
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
        table
            .WithColumn(nameof(ProductTemplate.Name)).AsString(400).NotNullable()
            .WithColumn(nameof(ProductTemplate.ViewPath)).AsString(400).NotNullable();
    }

    #endregion
}