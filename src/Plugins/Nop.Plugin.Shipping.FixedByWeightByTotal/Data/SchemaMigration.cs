using FluentMigrator;
using Nop.Data.Migrations;
using Nop.Plugin.Shipping.FixedByWeightByTotal.Domain;

namespace Nop.Plugin.Shipping.FixedByWeightByTotal.Data
{
    [SkipMigrationOnUpdate]
    [NopMigration("2020/02/03 08:40:55:1687541", "Shipping.FixedByWeightByTotal base schema")]
    public class SchemaMigration : AutoReversingMigration
    {
        protected IMigrationManager _migrationManager;

        public SchemaMigration(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }

        public override void Up()
        {
            _migrationManager.BuildTable<ShippingByWeightByTotalRecord>(Create);
        }
    }
}