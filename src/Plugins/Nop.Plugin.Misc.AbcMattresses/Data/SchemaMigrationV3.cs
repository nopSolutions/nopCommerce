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
    [NopMigration("2020/12/10 14:51:55:1687541", "Misc.AbcMattresses - moved TypeCategoryId, added BrandCategoryId")]
    public class SchemaMigrationV3 : Migration
    {
        protected IMigrationManager _migrationManager;

        public SchemaMigrationV3(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }

        public override void Up()
        {
            Alter.Table(nameof(AbcMattressModel)).AddColumn("TypeCategoryId")
                                                 .AsInt32()
                                                 .Nullable();
            Alter.Table(nameof(AbcMattressModel)).AddColumn(nameof(AbcMattressModel.BrandCategoryId))
                                                 .AsInt32()
                                                 .Nullable();

            Create.ForeignKey("FK_AbcMattressModel_Category_Type")
                  .FromTable(nameof(AbcMattressModel))
                  .ForeignColumn("TypeCategoryId")
                  .ToTable(nameof(Category))
                  .PrimaryColumn(nameof(Category.Id));
            Create.ForeignKey("FK_AbcMattressModel_Category_Brand")
                  .FromTable(nameof(AbcMattressModel))
                  .ForeignColumn(nameof(AbcMattressModel.BrandCategoryId))
                  .ToTable(nameof(Category))
                  .PrimaryColumn(nameof(Category.Id));
        }

        public override void Down()
        {
            Delete.ForeignKey("FK_AbcMattressModel_Category_Type")
                  .OnTable(nameof(AbcMattressModel));
            Delete.ForeignKey("FK_AbcMattressModel_Category_Brand")
                  .OnTable(nameof(AbcMattressModel));

            Delete.Column("TypeCategoryId").FromTable(nameof(AbcMattressModel));
            Delete.Column(nameof(AbcMattressModel.BrandCategoryId)).FromTable(nameof(AbcMattressModel));
        }
    }
}
