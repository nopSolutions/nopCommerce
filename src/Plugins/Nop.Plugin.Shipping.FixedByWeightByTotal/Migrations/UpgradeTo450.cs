using FluentMigrator;
using Nop.Data.Extensions;
using Nop.Data.Migrations;
using Nop.Plugin.Shipping.FixedByWeightByTotal.Domain;

namespace Nop.Plugin.Shipping.FixedByWeightByTotal.Migrations;

[NopMigration("2021-10-29 11:00:00", "Shipping.FixedByWeightByTotal change decimal precision", MigrationProcessType.Update)]
public class UpgradeTo450 : Migration
{
    public override void Up()
    {
        //ChangeDecimalPrecision
        this.AddOrAlterColumnFor<ShippingByWeightByTotalRecord>(t => t.WeightFrom)
            .AsDecimal(18, 4);
        this.AddOrAlterColumnFor<ShippingByWeightByTotalRecord>(t => t.WeightTo)
            .AsDecimal(18, 4);
        this.AddOrAlterColumnFor<ShippingByWeightByTotalRecord>(t => t.OrderSubtotalFrom)
            .AsDecimal(18, 4);
        this.AddOrAlterColumnFor<ShippingByWeightByTotalRecord>(t => t.OrderSubtotalTo)
            .AsDecimal(18, 4);
        this.AddOrAlterColumnFor<ShippingByWeightByTotalRecord>(t => t.AdditionalFixedCost)
            .AsDecimal(18, 4);
        this.AddOrAlterColumnFor<ShippingByWeightByTotalRecord>(t => t.PercentageRateOfSubtotal)
            .AsDecimal(18, 4);
        this.AddOrAlterColumnFor<ShippingByWeightByTotalRecord>(t => t.RatePerWeightUnit)
            .AsDecimal(18, 4);
        this.AddOrAlterColumnFor<ShippingByWeightByTotalRecord>(t => t.LowerWeightLimit)
            .AsDecimal(18, 4);
    }

    public override void Down()
    {
        //add the downgrade logic if necessary 
    }
}