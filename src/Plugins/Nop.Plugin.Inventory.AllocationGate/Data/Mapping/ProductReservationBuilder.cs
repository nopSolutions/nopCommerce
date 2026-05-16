using FluentMigrator.Builders.Create.Table;
using Nop.Data.Mapping.Builders;
using Nop.Plugin.Inventory.AllocationGate.Domain;

namespace Nop.Plugin.Inventory.AllocationGate.Data.Mapping;

public class ProductReservationBuilder : NopEntityBuilder<ProductReservation>
{
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
        table
            .WithColumn(nameof(ProductReservation.ReservationKey)).AsString(200).NotNullable().Unique()
            .WithColumn(nameof(ProductReservation.ChannelKey)).AsString(20).NotNullable()
            .WithColumn(nameof(ProductReservation.WarehouseId)).AsInt32().NotNullable().WithDefaultValue(0)
            .WithColumn(nameof(ProductReservation.ReservedUntilUtc)).AsDateTime2().Nullable();
    }
}
