using FluentMigrator;
using Nop.Data.Migrations;
using Nop.Plugin.Misc.AbcCore.Mattresses.Domain;

namespace Nop.Plugin.Misc.AbcCore.Data
{
    [NopMigration("2020/12/08 08:49:55:1687541", "Misc.AbcMattresses - added AbcMattressGift.Qty")]
    public class SchemaMigrationV2 : Migration
    {
        protected IMigrationManager _migrationManager;

        public SchemaMigrationV2(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }

        public override void Up()
        {
            Alter.Table(nameof(AbcMattressGift)).AddColumn("Qty").AsInt32();
        }

        public override void Down()
        {
            Delete.Column("Qty").FromTable(nameof(AbcMattressGift));
        }
    }
}
