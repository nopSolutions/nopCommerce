using FluentMigrator;
using Nop.Data.Migrations;
using Nop.Plugin.Misc.AbcCore.Domain;

namespace Nop.Plugin.Misc.AbcCore.Data
{
    [NopMigration("2020/10/21 15:12:55:1687541", "Misc.AbcCore base schema")]
    public class SchemaMigration : AutoReversingMigration
    {
        protected IMigrationManager _migrationManager;

        public SchemaMigration(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }

        public override void Up()
        {
            _migrationManager.BuildTable<AbcPromo>(Create);
            _migrationManager.BuildTable<AbcPromoProductMapping>(Create);
        }
    }
}
