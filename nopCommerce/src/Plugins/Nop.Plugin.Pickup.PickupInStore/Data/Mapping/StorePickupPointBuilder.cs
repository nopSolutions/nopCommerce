using FluentMigrator.Builders.Create.Table;
using Nop.Data.Mapping.Builders;
using Nop.Plugin.Pickup.PickupInStore.Domain;

namespace Nop.Plugin.Pickup.PickupInStore.Data.Mapping;

/// <summary>
/// Represents a pickup point entity builder
/// </summary>
public class StorePickupPointBuilder : NopEntityBuilder<StorePickupPoint>
{
    #region Methods

    /// <summary>
    /// Apply entity configuration
    /// </summary>
    /// <param name="table">Create table expression builder</param>
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
        table
            .WithColumn(nameof(StorePickupPoint.Latitude))
                .AsDecimal(18, 8)
                .Nullable()
            .WithColumn(nameof(StorePickupPoint.Longitude))
            .AsDecimal(18, 8)
                .Nullable();
    }

    #endregion
}
