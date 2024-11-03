using FluentMigrator;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;

namespace Nop.Data.Migrations.UpgradeTo480;

[NopSchemaMigration("2024-06-10 00:00:03", "SchemaMigration for 4.80.0")]
public class SchemaMigration : ForwardOnlyMigration
{
    /// <summary>
    /// Collect the UP migration expressions
    /// </summary>
    public override void Up()
    {
        //#7187
        var ptoductTableName = nameof(Product);
        var hasTierPricesColumnName = "HasTierPrices";
        if (Schema.Table(ptoductTableName).Column(hasTierPricesColumnName).Exists())
            Delete.Column(hasTierPricesColumnName).FromTable(ptoductTableName);

        //#7188
        var hasDiscountsAppliedColumnName = "HasDiscountsApplied";
        if (Schema.Table(ptoductTableName).Column(hasDiscountsAppliedColumnName).Exists())
            Delete.Column(hasDiscountsAppliedColumnName).FromTable(ptoductTableName);

        //#7241
        var categoryTableName = nameof(Category);
        var restrictFromVendorsColumnName = nameof(Category.RestrictFromVendors);

        if (!Schema.Table(categoryTableName).Column(restrictFromVendorsColumnName).Exists())
        {
            Alter.Table(categoryTableName)
                .AddColumn(restrictFromVendorsColumnName)
                .AsBoolean()
                .NotNullable()
                .WithDefaultValue(false);
        }

        //#7281
        var customerTableName = nameof(Customer);
        var mustChangePasswordColumnName = nameof(Customer.MustChangePassword);

        if (!Schema.Table(customerTableName).Column(mustChangePasswordColumnName).Exists())
        {
            Alter.Table(customerTableName)
                .AddColumn(mustChangePasswordColumnName)
                .AsBoolean()
                .NotNullable()
                .WithDefaultValue(false);
        }
    }
}
