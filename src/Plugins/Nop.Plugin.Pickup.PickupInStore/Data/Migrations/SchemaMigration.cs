using FluentMigrator;
using Nop.Data.Extensions;
using Nop.Data.Migrations;
using Nop.Plugin.Pickup.PickupInStore.Domain;

namespace Nop.Plugin.Pickup.PickupInStore.Data.Migrations;

[NopMigration("2020/02/03 09:30:17:6455422", "Pickup.PickupInStore base schema", MigrationProcessType.Installation)]
public class SchemaMigration : Migration
{
    /// <summary>
    /// Collect the UP migration expressions
    /// </summary>
    public override void Up()
    {
        this.CreateTableIfNotExists<StorePickupPoint>();
    }

    /// <summary>
    /// Collects the DOWN migration expressions
    /// </summary>
    public override void Down()
    {
        this.DeleteTableIfExists<StorePickupPoint>();
    }
}