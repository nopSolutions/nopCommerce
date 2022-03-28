using FluentMigrator;
using Nop.Data.Migrations;
using Nop.Plugin.Tax.AbcTax.Domain;

namespace Nop.Plugin.Tax.AbcTax.Data
{
    [SkipMigrationOnUpdate]
    [NopMigration("2021/09/15 09:27:23:6455432", "Tax.AbcTax base schema")]
    public class SchemaMigration : AutoReversingMigration
    {
        protected IMigrationManager _migrationManager;

        public SchemaMigration(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }

        public override void Up()
        {
            _migrationManager.BuildTable<AbcTaxRate>(Create);
        }
    }
}