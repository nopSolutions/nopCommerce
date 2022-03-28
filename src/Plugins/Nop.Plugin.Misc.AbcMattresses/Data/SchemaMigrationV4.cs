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
    [NopMigration("2020/12/10 15:23:55:1687541", "Misc.AbcMattresses - ItemNo to string")]
    public class SchemaMigrationV4 : Migration
    {
        protected IMigrationManager _migrationManager;

        public SchemaMigrationV4(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }

        public override void Up()
        {
            Alter.Column(nameof(AbcMattressBase.ItemNo)).OnTable(nameof(AbcMattressBase)).AsString();
            Alter.Column(nameof(AbcMattressEntry.ItemNo)).OnTable(nameof(AbcMattressEntry)).AsString();
            Alter.Column(nameof(AbcMattressGift.ItemNo)).OnTable(nameof(AbcMattressGift)).AsString();
            Alter.Column(nameof(AbcMattressPackage.ItemNo)).OnTable(nameof(AbcMattressPackage)).AsString();
        }

        public override void Down()
        {
            Alter.Column(nameof(AbcMattressBase.ItemNo)).OnTable(nameof(AbcMattressBase)).AsInt32();
            Alter.Column(nameof(AbcMattressEntry.ItemNo)).OnTable(nameof(AbcMattressEntry)).AsInt32();
            Alter.Column(nameof(AbcMattressGift.ItemNo)).OnTable(nameof(AbcMattressGift)).AsInt32();
            Alter.Column(nameof(AbcMattressPackage.ItemNo)).OnTable(nameof(AbcMattressPackage)).AsInt32();
        }
    }
}
