using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.PriceLists;
using Nop.Data.Extensions;

namespace Nop.Data.Mapping.Builders.PriceLists;

/// <summary>
/// Represents a price list item entity builder
/// </summary>
public partial class PriceListItemBuilder : NopEntityBuilder<PriceListItem>
{
    #region Methods

    /// <summary>
    /// Apply entity configuration
    /// </summary>
    /// <param name="table">Create table expression builder</param>
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
        table
            .WithColumn(nameof(PriceListItem.ProductId)).AsInt32().ForeignKey<Product>()
            .WithColumn(nameof(PriceListItem.PriceListId)).AsInt32().ForeignKey<PriceList>();
    }

    #endregion
}
