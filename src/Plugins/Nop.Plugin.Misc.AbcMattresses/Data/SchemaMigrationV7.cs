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
    [NopMigration("2020/12/26 20:21:55:1687541", "Misc.AbcMattresses - Added Frame schema")]
    public class SchemaMigrationV7 : AutoReversingMigration
    {
        protected IMigrationManager _migrationManager;

        public SchemaMigrationV7(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }

        public override void Up()
        {
            BuildAbcMattressFrame();
            BuildAbcMattressModelFrameMapping();
        }

        private void BuildAbcMattressFrame()
        {
            Create.Table(nameof(AbcMattressFrame))
                .WithColumn(nameof(AbcMattressFrame.Id)).AsInt32().NotNullable().PrimaryKey().Identity()
                .WithColumn(nameof(AbcMattressFrame.Name)).AsString()
                .WithColumn(nameof(AbcMattressFrame.ItemNo)).AsString()
                .WithColumn(nameof(AbcMattressFrame.Price)).AsDecimal();
        }

        private void BuildAbcMattressModelFrameMapping()
        {
            Create.Table("AbcMattressModelFrameMapping")
                .WithColumn("Id").AsInt32().NotNullable().PrimaryKey().Identity()
                .WithColumn("AbcMattressModelId")
                    .AsInt32()
                    .ForeignKey(nameof(AbcMattressModel), nameof(AbcMattressModel.Id))
                    .OnDelete(Rule.Cascade)
                .WithColumn("AbcMattressFrameId")
                    .AsInt32()
                    .ForeignKey(nameof(AbcMattressFrame), nameof(AbcMattressFrame.Id))
                    .OnDelete(Rule.Cascade);
        }
    }
}
