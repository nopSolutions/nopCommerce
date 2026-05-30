using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Catalog;
using Nop.Data.Extensions;

namespace Nop.Data.Mapping.Builders.Catalog;

/// <summary>
/// Represents a product product tag mapping entity builder
/// </summary>
public partial class ProductProductTagMappingBuilder : NopEntityBuilder<ProductProductTagMapping>
{
    #region Methods

    /// <summary>
    /// Apply entity configuration
    /// </summary>
    /// <param name="table">Create table expression builder</param>
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
        table
            .WithColumn(NameCompatibilityManager.GetColumnName(typeof(ProductProductTagMapping), nameof(ProductProductTagMapping.ProductId)))
            .AsInt32().PrimaryKey().ForeignKey<Product>()
            .WithColumn(NameCompatibilityManager.GetColumnName(typeof(ProductProductTagMapping), nameof(ProductProductTagMapping.ProductTagId)))
            .AsInt32().PrimaryKey().ForeignKey<ProductTag>();
    }

    #endregion
}