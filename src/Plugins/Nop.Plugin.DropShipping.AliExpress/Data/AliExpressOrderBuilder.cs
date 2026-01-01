using FluentMigrator.Builders.Create.Table;
using Nop.Data.Mapping.Builders;
using Nop.Plugin.DropShipping.AliExpress.Domain;

namespace Nop.Plugin.DropShipping.AliExpress.Data;

/// <summary>
/// Represents an AliExpress order entity builder
/// </summary>
public class AliExpressOrderBuilder : NopEntityBuilder<AliExpressOrder>
{
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
        table
            .WithColumn(nameof(AliExpressOrder.OrderId)).AsInt32().NotNullable()
            .WithColumn(nameof(AliExpressOrder.AliExpressOrderId)).AsInt64().Nullable()
            .WithColumn(nameof(AliExpressOrder.AliExpressProductId)).AsInt64().NotNullable()
            .WithColumn(nameof(AliExpressOrder.AliExpressOrderStatus)).AsString(100).Nullable()
            .WithColumn(nameof(AliExpressOrder.AliExpressTrackingNumber)).AsString(200).Nullable()
            .WithColumn(nameof(AliExpressOrder.LogisticsServiceName)).AsString(200).Nullable()
            .WithColumn(nameof(AliExpressOrder.PlacedOnUtc)).AsDateTime2().Nullable()
            .WithColumn(nameof(AliExpressOrder.ShippedOnUtc)).AsDateTime2().Nullable()
            .WithColumn(nameof(AliExpressOrder.DeliveredOnUtc)).AsDateTime2().Nullable()
            .WithColumn(nameof(AliExpressOrder.LocalShippingCreated)).AsBoolean().NotNullable()
            .WithColumn(nameof(AliExpressOrder.CourierGuyTrackingNumber)).AsString(200).Nullable()
            .WithColumn(nameof(AliExpressOrder.CreatedOnUtc)).AsDateTime2().NotNullable()
            .WithColumn(nameof(AliExpressOrder.UpdatedOnUtc)).AsDateTime2().NotNullable()
            .WithColumn(nameof(AliExpressOrder.LastErrorMessage)).AsString(1000).Nullable()
            .WithColumn(nameof(AliExpressOrder.OrderDetailsJson)).AsString(int.MaxValue).Nullable();
    }
}
