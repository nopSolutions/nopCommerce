using System.Data;
using FluentMigrator;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Topics;
using Nop.Core.Domain.Vendors;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.UpgradeTo480;

[NopSchemaMigration("2024-11-18 00:00:00", "SchemaMigration for 4.80.0")]
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

        //#7242
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

        //#7294
        var topicTableName = nameof(Topic);
        var topicSystemNameColumnName = nameof(Topic.SystemName);

        if (!Schema.Table(topicTableName).Index("IX_Topic_SystemName").Exists())
        {
            Alter.Table(topicTableName)
                .AlterColumn(topicSystemNameColumnName)
                .AsString(400)
                .Nullable();
        }

        //#7241
        var discountTableName = nameof(Discount);
        var vendorIdDiscountColumnName = nameof(Discount.VendorId);

        if (!Schema.Table(discountTableName).Column(vendorIdDiscountColumnName).Exists())
        {
            Alter.Table(discountTableName)
                .AddColumn(vendorIdDiscountColumnName).AsInt32().ForeignKey<Vendor>(onDelete: Rule.SetNull).Nullable();
        }

        //#7243
        var vendorTableName = nameof(Vendor);
        var pmCustomerIdColumnName = nameof(Vendor.PmCustomerId);
        if (!Schema.Table(vendorTableName).Column(pmCustomerIdColumnName).Exists())
            Alter.Table(vendorTableName).AddColumn(pmCustomerIdColumnName).AsInt32().Nullable();
    }
}
