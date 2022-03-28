using FluentMigrator;
using Nop.Data.Migrations;
using Nop.Plugin.Misc.AbcCore.Mattresses.Domain;

namespace Nop.Plugin.Misc.AbcCore.Data
{
    [NopMigration("2020/12/16 11:21:55:1687541", "Misc.AbcMattresses - Added Package Base Qty")]
    public class SchemaMigrationV5 : AutoReversingMigration
    {
        protected IMigrationManager _migrationManager;

        public SchemaMigrationV5(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }

        public override void Up()
        {
            Create.Column(nameof(AbcMattressPackage.BaseQuantity))
                  .OnTable(nameof(AbcMattressPackage))
                  .AsInt16()
                  .WithDefaultValue(1);
        }
    }
}
