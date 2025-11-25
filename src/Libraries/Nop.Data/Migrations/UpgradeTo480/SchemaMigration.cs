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
        var hasTierPricesColumnName = "HasTierPrices";
        if (Schema.ColumnExist<Product>(hasTierPricesColumnName))
            Delete.Column<Product>(hasTierPricesColumnName);

        //#7188
        var hasDiscountsAppliedColumnName = "HasDiscountsApplied";
        if (Schema.ColumnExist<Product>(hasDiscountsAppliedColumnName))
            Delete.Column<Product>(hasDiscountsAppliedColumnName);

        //#7242

        if (!Schema.ColumnExist<Category>(t => t.RestrictFromVendors))
        {
            Alter.AddColumnFor<Category>(t => t.RestrictFromVendors)
                .AsBoolean()
                .NotNullable()
                .WithDefaultValue(false);
        }

        //#7281

        if (!Schema.ColumnExist<Customer>(t => t.MustChangePassword))
        {
            Alter.AddColumnFor<Customer>(t => t.MustChangePassword)
                .AsBoolean()
                .NotNullable()
                .WithDefaultValue(false);
        }

        //#7294

        if (!Schema.TableFor<Topic>().Index("IX_Topic_SystemName").Exists())
        {
            Alter.AlterColumnFor<Topic>(t => t.SystemName)
                .AsString(400)
                .Nullable();
        }

        //#7241

        if (!Schema.ColumnExist<Discount>(t => t.VendorId))
        {
            Alter.AddColumnFor<Discount>(t => t.VendorId)
                .AsInt32()
                .ForeignKey<Vendor>(onDelete: Rule.SetNull)
                .Nullable();
        }

        //#7243
        if (!Schema.ColumnExist<Vendor>(t => t.PmCustomerId))
        {
            Alter.AddColumnFor<Vendor>(t => t.PmCustomerId)
                .AsInt32()
                .Nullable();
        }
    }
}
