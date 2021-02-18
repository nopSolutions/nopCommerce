using FluentMigrator;
using Nop.Data.Migrations;
using Nop.Plugin.Pickup.PickupInStore.Domain;

namespace Nop.Plugin.Pickup.PickupInStore.Data
{
    [SkipMigrationOnUpdate]
    [NopMigration("2020/02/03 09:30:17:6455422", "Pickup.PickupInStore base schema")]
    public class SchemaMigration : AutoReversingMigration
    {
        protected IMigrationManager _migrationManager;

        public SchemaMigration(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }

        public override void Up()
        {
            _migrationManager.BuildTable<StorePickupPoint>(Create);
        }
    }
}