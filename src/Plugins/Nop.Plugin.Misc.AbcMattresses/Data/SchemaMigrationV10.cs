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
    [NopMigration("2021/01/05 09:39:55:1687541", "Misc.AbcMattresses - Modified AbcMattressFrames")]
    public class SchemaMigrationV10 : Migration
    {
        protected IMigrationManager _migrationManager;

        public SchemaMigrationV10(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }

        public override void Up()
        {
            Delete.Table("AbcMattressModelFrameMapping");
            Create.Column(nameof(AbcMattressFrame.Size))
                  .OnTable(nameof(AbcMattressFrame))
                  .AsString()
                  .WithDefaultValue(" ");
        }

        public override void Down()
        {
            Delete.Column(nameof(AbcMattressFrame.Size)).FromTable(nameof(AbcMattressFrame));
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
