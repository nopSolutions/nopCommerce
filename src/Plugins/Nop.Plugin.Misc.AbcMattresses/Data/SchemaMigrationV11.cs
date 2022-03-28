using FluentMigrator;
using Nop.Data.Migrations;
using Nop.Plugin.Misc.AbcCore.Mattresses.Domain;

namespace Nop.Plugin.Misc.AbcCore.Data
{
    [NopMigration("2021/02/17 09:39:55:1687541", "Misc.AbcMattresses - Add Sku to AbcMattressModel")]
    public class SchemaMigrationV11 : AutoReversingMigration
    {
        protected IMigrationManager _migrationManager;

        public SchemaMigrationV11(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }

        public override void Up()
        {
            Create.Column(nameof(AbcMattressModel.Sku))
                  .OnTable(nameof(AbcMattressModel))
                  .AsString()
                  .Nullable();
        }
    }
}
