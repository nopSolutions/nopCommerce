using FluentMigrator;
using Nop.Data.Migrations;

namespace Nop.Plugin.Inventory.AllocationGate.Data.Migrations;

[NopMigration("2026/05/15 12:00:00", "Nop.Plugin.Inventory.AllocationGate add WarehouseId", MigrationProcessType.Update)]
public class AddWarehouseIdMigration : Migration
{
    public override void Up()
    {
        if (!Schema.Table("ProductReservation").Column("WarehouseId").Exists())
            Alter.Table("ProductReservation")
                .AddColumn("WarehouseId").AsInt32().NotNullable().WithDefaultValue(0);
    }

    public override void Down()
    {
        if (Schema.Table("ProductReservation").Column("WarehouseId").Exists())
            Delete.Column("WarehouseId").FromTable("ProductReservation");
    }
}
