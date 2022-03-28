using System;
using FluentMigrator;
using Nop.Data.Migrations;
using Nop.Plugin.Misc.AbcCore.Mattresses.Domain;
using Nop.Core.Domain.Catalog;
using System.Data;

namespace Nop.Plugin.Misc.AbcCore.Data
{
    [NopMigration("2020/12/06 11:44:55:1687541", "Misc.AbcMattresses base schema")]
    public class SchemaMigration : AutoReversingMigration
    {
        protected IMigrationManager _migrationManager;

        public SchemaMigration(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }

        public override void Up()
        {
            BuildAbcMattressModel();
            BuildAbcMattressEntry();
            BuildAbcMattressGift();
            BuildAbcMattressBase();
            BuildAbcMattressPackage();
            BuildAbcMattressModelGiftMapping();
        }

        private void BuildAbcMattressModel()
        {
            Create.Table(nameof(AbcMattressModel))
                .WithColumn(nameof(AbcMattressModel.Id)).AsInt32().NotNullable().PrimaryKey().Identity()
                .WithColumn(nameof(AbcMattressModel.Name)).AsString()
                .WithColumn(nameof(AbcMattressModel.ManufacturerId))
                    .AsInt32()
                    .Nullable()
                    .ForeignKey(nameof(Manufacturer), nameof(Manufacturer.Id))
                    .OnDelete(Rule.SetNull)
                .WithColumn(nameof(AbcMattressModel.Description)).AsString()
                .WithColumn(nameof(AbcMattressModel.Comfort)).AsString()
                .WithColumn(nameof(AbcMattressModel.ProductId))
                    .AsInt32()
                    .Nullable()
                    .ForeignKey(nameof(Product), nameof(Product.Id))
                    .OnDelete(Rule.SetNull);
        }

        private void BuildAbcMattressEntry()
        {
            Create.Table(nameof(AbcMattressEntry))
                .WithColumn(nameof(AbcMattressEntry.Id)).AsInt32().NotNullable().PrimaryKey().Identity()
                .WithColumn(nameof(AbcMattressEntry.AbcMattressModelId))
                    .AsInt32()
                    .ForeignKey(nameof(AbcMattressModel), nameof(AbcMattressModel.Id))
                    .OnDelete(Rule.Cascade)
                .WithColumn(nameof(AbcMattressEntry.Size)).AsString()
                .WithColumn(nameof(AbcMattressEntry.ItemNo)).AsString()
                .WithColumn(nameof(AbcMattressEntry.Price)).AsDecimal();
        }

        private void BuildAbcMattressGift()
        {
            Create.Table(nameof(AbcMattressGift))
                .WithColumn(nameof(AbcMattressGift.Id)).AsInt32().NotNullable().PrimaryKey().Identity()
                .WithColumn(nameof(AbcMattressGift.ItemNo)).AsString()
                .WithColumn(nameof(AbcMattressGift.Description)).AsString()
                .WithColumn(nameof(AbcMattressGift.Amount)).AsDecimal();
        }

        private void BuildAbcMattressBase()
        {
            Create.Table(nameof(AbcMattressBase))
                .WithColumn(nameof(AbcMattressBase.Id)).AsInt32().NotNullable().PrimaryKey().Identity()
                .WithColumn(nameof(AbcMattressBase.ItemNo)).AsString()
                .WithColumn(nameof(AbcMattressBase.Name)).AsString()
                .WithColumn("Price").AsDecimal()
                .WithColumn(nameof(AbcMattressBase.IsAdjustable)).AsBoolean();
        }

        private void BuildAbcMattressPackage()
        {
            Create.Table(nameof(AbcMattressPackage))
                .WithColumn(nameof(AbcMattressPackage.Id)).AsInt32().NotNullable().PrimaryKey().Identity()
                .WithColumn(nameof(AbcMattressPackage.AbcMattressEntryId))
                    .AsInt32()
                    .ForeignKey(nameof(AbcMattressEntry), nameof(AbcMattressEntry.Id))
                    .OnDelete(Rule.Cascade)
                .WithColumn(nameof(AbcMattressPackage.AbcMattressBaseId))
                    .AsInt32()
                    .ForeignKey(nameof(AbcMattressBase), nameof(AbcMattressBase.Id))
                    .OnDelete(Rule.Cascade)
                .WithColumn(nameof(AbcMattressPackage.ItemNo)).AsString()
                .WithColumn(nameof(AbcMattressPackage.Price)).AsDecimal();
        }

        private void BuildAbcMattressModelGiftMapping()
        {
            Create.Table(nameof(AbcMattressModelGiftMapping))
                .WithColumn(nameof(AbcMattressModelGiftMapping.Id)).AsInt32().NotNullable().PrimaryKey().Identity()
                .WithColumn(nameof(AbcMattressModelGiftMapping.AbcMattressModelId))
                    .AsInt32()
                    .ForeignKey(nameof(AbcMattressModel), nameof(AbcMattressModel.Id))
                    .OnDelete(Rule.Cascade)
                .WithColumn(nameof(AbcMattressModelGiftMapping.AbcMattressGiftId))
                    .AsInt32()
                    .ForeignKey(nameof(AbcMattressGift), nameof(AbcMattressGift.Id))
                    .OnDelete(Rule.Cascade);
        }
    }
}
