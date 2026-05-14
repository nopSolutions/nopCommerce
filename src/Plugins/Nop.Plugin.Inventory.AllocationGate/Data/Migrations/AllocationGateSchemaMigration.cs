using FluentMigrator;
using Nop.Data.Extensions;
using Nop.Data.Migrations;
using Nop.Plugin.Inventory.AllocationGate.Domain;

namespace Nop.Plugin.Inventory.AllocationGate.Data.Migrations;

[NopMigration("2026/05/13 12:00:00", "Nop.Plugin.Inventory.AllocationGate base schema", MigrationProcessType.Installation)]
public class AllocationGateSchemaMigration : Migration
{
    public override void Up()
    {
        this.CreateTableIfNotExists<ProductReservation>();
    }

    public override void Down()
    {
        this.DeleteTableIfExists<ProductReservation>();
    }
}
