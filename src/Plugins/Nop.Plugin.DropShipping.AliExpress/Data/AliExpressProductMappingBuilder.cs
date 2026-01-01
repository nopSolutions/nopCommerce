using FluentMigrator.Builders.Create.Table;
using Nop.Data.Mapping.Builders;
using Nop.Plugin.DropShipping.AliExpress.Domain;

namespace Nop.Plugin.DropShipping.AliExpress.Data;

/// <summary>
/// Represents an AliExpress product mapping entity builder
/// </summary>
public class AliExpressProductMappingBuilder : NopEntityBuilder<AliExpressProductMapping>
{
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
        table
            .WithColumn(nameof(AliExpressProductMapping.ProductId)).AsInt32().NotNullable()
            .WithColumn(nameof(AliExpressProductMapping.AliExpressProductId)).AsInt64().NotNullable()
            .WithColumn(nameof(AliExpressProductMapping.AliExpressProductUrl)).AsString(500).Nullable()
            .WithColumn(nameof(AliExpressProductMapping.SkuAttributes)).AsString(500).Nullable()
            .WithColumn(nameof(AliExpressProductMapping.AliExpressPrice)).AsDecimal(18, 4).NotNullable()
            .WithColumn(nameof(AliExpressProductMapping.ShippingCost)).AsDecimal(18, 4).NotNullable()
            .WithColumn(nameof(AliExpressProductMapping.VatAmount)).AsDecimal(18, 4).NotNullable()
            .WithColumn(nameof(AliExpressProductMapping.CustomsDuty)).AsDecimal(18, 4).NotNullable()
            .WithColumn(nameof(AliExpressProductMapping.MarginPercentage)).AsDecimal(18, 4).NotNullable()
            .WithColumn(nameof(AliExpressProductMapping.CalculatedPrice)).AsDecimal(18, 4).NotNullable()
            .WithColumn(nameof(AliExpressProductMapping.ShippingServiceName)).AsString(200).Nullable()
            .WithColumn(nameof(AliExpressProductMapping.EstimatedDeliveryDays)).AsInt32().Nullable()
            .WithColumn(nameof(AliExpressProductMapping.CreatedOnUtc)).AsDateTime2().NotNullable()
            .WithColumn(nameof(AliExpressProductMapping.LastSyncOnUtc)).AsDateTime2().NotNullable()
            .WithColumn(nameof(AliExpressProductMapping.IsAvailable)).AsBoolean().NotNullable()
            .WithColumn(nameof(AliExpressProductMapping.LastSyncMessage)).AsString(1000).Nullable()
            .WithColumn(nameof(AliExpressProductMapping.ProductDetailsJson)).AsString(int.MaxValue).Nullable();
    }
}
