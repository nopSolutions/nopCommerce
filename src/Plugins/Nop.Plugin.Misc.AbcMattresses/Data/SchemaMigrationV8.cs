using FluentMigrator;
using Nop.Data.Migrations;
using Nop.Plugin.Misc.AbcCore.Mattresses.Domain;
using FluentMigrator.Builders.Create.Table;
using Nop.Data.Mapping.Builders;
using Nop.Data.Extensions;
using System.Data;
using Nop.Core.Domain.Catalog;

namespace Nop.Plugin.Misc.AbcCore.Data
{
    [NopMigration("2020/12/28 12:21:55:1687541", "Misc.AbcMattresses - Removed Base price")]
    public class SchemaMigrationV8 : Migration
    {
        protected IMigrationManager _migrationManager;

        public SchemaMigrationV8(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }

        public override void Up()
        {
            Delete.Column("Price")
                  .FromTable(nameof(AbcMattressBase));
        }

        public override void Down()
        {
            Create.Column("Price")
                  .OnTable(nameof(AbcMattressBase))
                  .AsDecimal()
                  .Nullable();
        }
    }
}
