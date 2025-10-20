using FluentMigrator;
using FluentMigrator.SqlServer;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Topics;
using Nop.Data.Mapping;

namespace Nop.Data.Migrations.UpgradeTo490;

[NopSchemaMigration("2025-01-28 00:00:01", "AddIndexesMigration for 4.90.0")]
public class AddIndexesMigration : ForwardOnlyMigration
{
    /// <summary>
    /// Collect the UP migration expressions
    /// </summary>
    public override void Up()
    {
        //#7296
        var topicTableName = nameof(Topic);
        var topicEndDateColumnName = nameof(Topic.AvailableEndDateTimeUtc);
        var topicStartDateColumnName = nameof(Topic.AvailableStartDateTimeUtc);
        var topicAvailableDatesIndexName = "IX_Topic_Availability";
        if (!Schema.Table(topicTableName).Index(topicAvailableDatesIndexName).Exists())
            Create.Index(topicAvailableDatesIndexName)
                .OnTable(topicTableName)
                .OnColumn(topicEndDateColumnName).Descending()
                .OnColumn(topicStartDateColumnName).Descending()
                .WithOptions().NonClustered();

        //#7676
        const string databaseType = "sqlserver";

        var productTableName = nameof(Product);
        var nameColumnName = nameof(Product.Name);
        var skuColumnName = nameof(Product.Sku);
        var manufacturerPartNumberColumnName = nameof(Product.ManufacturerPartNumber);
        var idColumnName = nameof(Product.Id);
        var deletedColumnName = nameof(Product.Deleted);
        var productSearchIndexName = "IX_Product_Search";
        if (!Schema.Table(productTableName).Index(productSearchIndexName).Exists())
            IfDatabase(databaseType).Create.Index(productSearchIndexName)
                .OnTable(productTableName)
                .OnColumn(nameColumnName).Ascending()
                .OnColumn(skuColumnName).Ascending()
                .OnColumn(manufacturerPartNumberColumnName).Ascending()
                .OnColumn(deletedColumnName).Ascending()
                .WithOptions().NonClustered()
                .Include(idColumnName);

        var productAttributeCombinationTableName = nameof(ProductAttributeCombination);
        skuColumnName = nameof(ProductAttributeCombination.Sku);
        var productIdColumnName = nameof(ProductAttributeCombination.ProductId);
        var productAttributeCombinationSkuIndexName = "IX_ProductAttributeCombination_Sku";
        if (!Schema.Table(productAttributeCombinationTableName).Index(productAttributeCombinationSkuIndexName).Exists())
            Create.Index(productAttributeCombinationSkuIndexName)
                .OnTable(productAttributeCombinationTableName)
                .OnColumn(skuColumnName).Ascending()
                .WithOptions().NonClustered()
                .Include(productIdColumnName);

        var categoryTableName = nameof(Category);
        nameColumnName = nameof(Category.Name);
        deletedColumnName = nameof(Category.Deleted);
        idColumnName = nameof(Category.Id);
        var categoryNameDeletedIndexName = "IX_Category_Name_Deleted";
        if (!Schema.Table(categoryTableName).Index(categoryNameDeletedIndexName).Exists())
            Create.Index(categoryNameDeletedIndexName)
                .OnTable(categoryTableName)
                .OnColumn(nameColumnName).Ascending()
                .OnColumn(deletedColumnName).Ascending()
                .WithOptions().NonClustered()
                .Include(idColumnName);

        var manufacturerTableName = nameof(Manufacturer);
        nameColumnName = nameof(Manufacturer.Name);
        deletedColumnName = nameof(Manufacturer.Deleted);
        idColumnName = nameof(Manufacturer.Id);
        var manufacturerNameDeletedIndexName = "IX_Manufacturer_Name_Deleted";
        if (!Schema.Table(manufacturerTableName).Index(manufacturerNameDeletedIndexName).Exists())
            Create.Index(manufacturerNameDeletedIndexName)
                .OnTable(manufacturerTableName)
                .OnColumn(nameColumnName).Ascending()
                .OnColumn(deletedColumnName).Ascending()
                .WithOptions().NonClustered()
                .Include(idColumnName);

        var productCategoryTableName = NameCompatibilityManager.GetTableName(typeof(ProductCategory));
        var categoryIdColumnName = nameof(ProductCategory.CategoryId);
        productIdColumnName = nameof(ProductCategory.ProductId);
        var productCategoryMappingIndexName = "IX_ProductCategoryMapping_CategoryId";
        if (!Schema.Table(productCategoryTableName).Index(productCategoryMappingIndexName).Exists())
            Create.Index(productCategoryMappingIndexName)
                .OnTable(productCategoryTableName)
                .OnColumn(categoryIdColumnName).Ascending()
                .WithOptions().NonClustered()
                .Include(productIdColumnName);

        var productManufacturerTableName = NameCompatibilityManager.GetTableName(typeof(ProductManufacturer));
        var manufacturerIdColumnName = nameof(ProductManufacturer.ManufacturerId);
        productIdColumnName = nameof(ProductManufacturer.ProductId);
        var productManufacturerMappingIndexName = "IX_ProductManufacturerMapping_ManufacturerId";
        if (!Schema.Table(productManufacturerTableName).Index(productManufacturerMappingIndexName).Exists())
            Create.Index(productManufacturerMappingIndexName)
                .OnTable(productManufacturerTableName)
                .OnColumn(manufacturerIdColumnName).Ascending()
                .WithOptions().NonClustered()
                .Include(productIdColumnName);
    }
}