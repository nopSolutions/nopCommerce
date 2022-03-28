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
    [NopMigration("2020/12/29 09:39:55:1687541", "Misc.AbcMattresses - Added AbcMattressProtector schema")]
    public class SchemaMigrationV9 : AutoReversingMigration
    {
        protected IMigrationManager _migrationManager;

        public SchemaMigrationV9(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }

        public override void Up()
        {
            Create.Table(nameof(AbcMattressProtector))
                .WithColumn(nameof(AbcMattressProtector.Id)).AsInt32().NotNullable().PrimaryKey().Identity()
                .WithColumn(nameof(AbcMattressProtector.Size)).AsString()
                .WithColumn(nameof(AbcMattressProtector.Name)).AsString()
                .WithColumn(nameof(AbcMattressProtector.ItemNo)).AsString()
                .WithColumn(nameof(AbcMattressProtector.Price)).AsDecimal();
        }
    }
}
