using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.PriceLists;

namespace Nop.Data.Mapping.Builders.PriceLists;

/// <summary>
/// Represents a price list entity builder
/// </summary>
public partial class PriceListBuilder : NopEntityBuilder<PriceList>
{
    #region Methods

    /// <summary>
    /// Apply entity configuration
    /// </summary>
    /// <param name="table">Create table expression builder</param>
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
        table
            .WithColumn(nameof(PriceList.Name)).AsString(1000).Nullable()
            .WithColumn(nameof(PriceList.Description)).AsString(1000).Nullable()
            .WithColumn(nameof(PriceList.StartDateUtc)).AsDateTime2().Nullable()
            .WithColumn(nameof(PriceList.EndDateUtc)).AsDateTime2().Nullable()
            .WithColumn(nameof(PriceList.PriceCalculationTypeId)).AsInt32();
    }

    #endregion
}
