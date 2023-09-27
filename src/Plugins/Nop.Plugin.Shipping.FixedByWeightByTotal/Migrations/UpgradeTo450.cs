using FluentMigrator;
using Nop.Data.Migrations;
using Nop.Plugin.Shipping.FixedByWeightByTotal.Domain;

namespace Nop.Plugin.Shipping.FixedByWeightByTotal.Migrations
{
    [NopMigration("2021-10-29 11:00:00", "Shipping.FixedByWeightByTotal change decimal precision", MigrationProcessType.Update)]
    public class ChangeDecimalPrecision : Migration
    {
        public override void Up()
        {
            var tableName = nameof(ShippingByWeightByTotalRecord);

            foreach (var columnName in new[]
            {
                nameof(ShippingByWeightByTotalRecord.WeightFrom),
                nameof(ShippingByWeightByTotalRecord.WeightTo),
                nameof(ShippingByWeightByTotalRecord.OrderSubtotalFrom),
                nameof(ShippingByWeightByTotalRecord.OrderSubtotalTo),
                nameof(ShippingByWeightByTotalRecord.AdditionalFixedCost),
                nameof(ShippingByWeightByTotalRecord.PercentageRateOfSubtotal),
                nameof(ShippingByWeightByTotalRecord.RatePerWeightUnit),
                nameof(ShippingByWeightByTotalRecord.LowerWeightLimit)
            })
            {
                if (!Schema.Table(tableName).Column(columnName).Exists())
                    continue;

                Alter.Table(tableName)
                    .AlterColumn(columnName).AsDecimal(18, 4);
            }
        }

        public override void Down()
        {
            //add the downgrade logic if necessary 
        }
    }
}
