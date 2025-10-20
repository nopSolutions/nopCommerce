using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.FilterLevels;
using Nop.Data.Extensions;

namespace Nop.Data.Mapping.Builders.FilterLevels;

/// <summary>
/// Represents a filter level value and product mapping entity builder
/// </summary>
public partial class FilterLevelValueProductMappingBuilder : NopEntityBuilder<FilterLevelValueProductMapping>
{
    #region Methods

    /// <summary>
    /// Apply entity configuration
    /// </summary>
    /// <param name="table">Create table expression builder</param>
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
        table
            .WithColumn(nameof(FilterLevelValueProductMapping.FilterLevelValueId)).AsInt32().ForeignKey<FilterLevelValue>()
            .WithColumn(nameof(FilterLevelValueProductMapping.ProductId)).AsInt32().ForeignKey<Product>();
    }

    #endregion
}
