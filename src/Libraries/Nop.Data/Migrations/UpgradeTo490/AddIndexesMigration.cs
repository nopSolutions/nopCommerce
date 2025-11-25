using FluentMigrator;
using FluentMigrator.SqlServer;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Topics;
using Nop.Data.Extensions;
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
        var topicTableName = NameCompatibilityManager.GetTableName(typeof(Topic));
        var topicEndDateColumnName = NameCompatibilityManager.GetColumnName(typeof(Topic), nameof(Topic.AvailableEndDateTimeUtc));
        var topicStartDateColumnName = NameCompatibilityManager.GetColumnName(typeof(Topic), nameof(Topic.AvailableStartDateTimeUtc));
        ;
        var topicAvailableDatesIndexName = "IX_Topic_Availability";
        if (!Schema.TableFor<Topic>().Index(topicAvailableDatesIndexName).Exists())
        {
            Create.Index(topicAvailableDatesIndexName)
                .OnTable(topicTableName)
                .OnColumn(topicEndDateColumnName).Descending()
                .OnColumn(topicStartDateColumnName).Descending()
                .WithOptions().NonClustered();
        }

        //#7676
        const string databaseType = "sqlserver";

        var productTableName = NameCompatibilityManager.GetTableName(typeof(Product));
        var nameColumnName = NameCompatibilityManager.GetColumnName(typeof(Product), nameof(Product.Name));
        var skuColumnName = NameCompatibilityManager.GetColumnName(typeof(Product), nameof(Product.Sku));
        var manufacturerPartNumberColumnName = NameCompatibilityManager.GetColumnName(typeof(Product), nameof(Product.ManufacturerPartNumber));
        var idColumnName = NameCompatibilityManager.GetColumnName(typeof(Product), nameof(Product.Id));
        var deletedColumnName = NameCompatibilityManager.GetColumnName(typeof(Product), nameof(Product.Deleted));
        var productSearchIndexName = "IX_Product_Search";
        if (!Schema.TableFor<Product>().Index(productSearchIndexName).Exists())
            IfDatabase(databaseType).Create.Index(productSearchIndexName)
                .OnTable(productTableName)
                .OnColumn(nameColumnName).Ascending()
                .OnColumn(skuColumnName).Ascending()
                .OnColumn(manufacturerPartNumberColumnName).Ascending()
                .OnColumn(deletedColumnName).Ascending()
                .WithOptions().NonClustered()
                .Include(idColumnName);

        var productAttributeCombinationTableName = NameCompatibilityManager.GetTableName(typeof(ProductAttributeCombination));
        skuColumnName = NameCompatibilityManager.GetColumnName(typeof(ProductAttributeCombination), nameof(ProductAttributeCombination.Sku));
        var productIdColumnName = NameCompatibilityManager.GetColumnName(typeof(ProductAttributeCombination), nameof(ProductAttributeCombination.ProductId));
        var productAttributeCombinationSkuIndexName = "IX_ProductAttributeCombination_Sku";
        if (!Schema.TableFor<ProductAttributeCombination>().Index(productAttributeCombinationSkuIndexName).Exists())
        {
            Create.Index(productAttributeCombinationSkuIndexName)
                .OnTable(productAttributeCombinationTableName)
                .OnColumn(skuColumnName).Ascending()
                .WithOptions().NonClustered()
                .Include(productIdColumnName);
        }

        var categoryTableName = NameCompatibilityManager.GetTableName(typeof(Category));
        nameColumnName = NameCompatibilityManager.GetColumnName(typeof(Category), nameof(Category.Name));
        deletedColumnName = NameCompatibilityManager.GetColumnName(typeof(Category), nameof(Category.Deleted));
        idColumnName = NameCompatibilityManager.GetColumnName(typeof(Category), nameof(Category.Id));
        var categoryNameDeletedIndexName = "IX_Category_Name_Deleted";
        if (!Schema.TableFor<Category>().Index(categoryNameDeletedIndexName).Exists())
        {
            Create.Index(categoryNameDeletedIndexName)
                .OnTable(categoryTableName)
                .OnColumn(nameColumnName).Ascending()
                .OnColumn(deletedColumnName).Ascending()
                .WithOptions().NonClustered()
                .Include(idColumnName);
        }

        var manufacturerTableName = NameCompatibilityManager.GetTableName(typeof(Manufacturer));
        nameColumnName = NameCompatibilityManager.GetColumnName(typeof(Manufacturer), nameof(Manufacturer.Name));
        deletedColumnName = NameCompatibilityManager.GetColumnName(typeof(Manufacturer), nameof(Manufacturer.Deleted));
        idColumnName = NameCompatibilityManager.GetColumnName(typeof(Manufacturer), nameof(Manufacturer.Id));
        var manufacturerNameDeletedIndexName = "IX_Manufacturer_Name_Deleted";
        if (!Schema.TableFor<Manufacturer>().Index(manufacturerNameDeletedIndexName).Exists())
        {
            Create.Index(manufacturerNameDeletedIndexName)
                .OnTable(manufacturerTableName)
                .OnColumn(nameColumnName).Ascending()
                .OnColumn(deletedColumnName).Ascending()
                .WithOptions().NonClustered()
                .Include(idColumnName);
        }

        var productCategoryTableName = NameCompatibilityManager.GetTableName(typeof(ProductCategory));
        var categoryIdColumnName = NameCompatibilityManager.GetColumnName(typeof(ProductCategory), nameof(ProductCategory.CategoryId));
        productIdColumnName = NameCompatibilityManager.GetColumnName(typeof(ProductCategory), nameof(ProductCategory.ProductId));
        var productCategoryMappingIndexName = "IX_ProductCategoryMapping_CategoryId";
        if (!Schema.TableFor<ProductCategory>().Index(productCategoryMappingIndexName).Exists())
        {
            Create.Index(productCategoryMappingIndexName)
                .OnTable(productCategoryTableName)
                .OnColumn(categoryIdColumnName).Ascending()
                .WithOptions().NonClustered()
                .Include(productIdColumnName);
        }

        var productManufacturerTableName = NameCompatibilityManager.GetTableName(typeof(ProductManufacturer));
        var manufacturerIdColumnName = NameCompatibilityManager.GetColumnName(typeof(ProductManufacturer), nameof(ProductManufacturer.ManufacturerId));
        productIdColumnName = NameCompatibilityManager.GetColumnName(typeof(ProductManufacturer), nameof(ProductManufacturer.ProductId));
        var productManufacturerMappingIndexName = "IX_ProductManufacturerMapping_ManufacturerId";
        if (!Schema.TableFor<ProductManufacturer>().Index(productManufacturerMappingIndexName).Exists())
        {
            Create.Index(productManufacturerMappingIndexName)
                .OnTable(productManufacturerTableName)
                .OnColumn(manufacturerIdColumnName).Ascending()
                .WithOptions().NonClustered()
                .Include(productIdColumnName);
        }
    }
}