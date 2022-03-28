using FluentMigrator;
using Nop.Core.Domain.Catalog;
using Nop.Data.Migrations;
using Nop.Plugin.Misc.AbcCore.Mattresses.Domain;

namespace Nop.Plugin.Misc.AbcCore.Data
{
    [NopMigration("2020/12/26 11:21:55:1687541", "Misc.AbcMattresses - Moving Type")]
    public class SchemaMigrationV6 : Migration
    {
        protected IMigrationManager _migrationManager;

        public SchemaMigrationV6(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }

        public override void Up()
        {
            Create.Column(nameof(AbcMattressEntry.Type))
                  .OnTable(nameof(AbcMattressEntry))
                  .AsString()
                  .Nullable();

            Delete.ForeignKey("FK_AbcMattressModel_Category_Type")
                  .OnTable(nameof(AbcMattressModel));
            Delete.Column("TypeCategoryId")
                  .FromTable(nameof(AbcMattressModel));
        }

        public override void Down()
        {
            Alter.Table(nameof(AbcMattressModel)).AddColumn("TypeCategoryId")
                                                 .AsInt32()
                                                 .Nullable();
            Create.ForeignKey("FK_AbcMattressModel_Category_Type")
                  .FromTable(nameof(AbcMattressModel))
                  .ForeignColumn("TypeCategoryId")
                  .ToTable(nameof(Category))
                  .PrimaryColumn(nameof(Category.Id));

            Delete.Column(nameof(AbcMattressEntry.Type))
                  .FromTable(nameof(AbcMattressEntry));
        }
    }
}
