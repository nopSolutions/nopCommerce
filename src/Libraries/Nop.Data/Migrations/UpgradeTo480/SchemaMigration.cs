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
        //#7188
        this.DeleteColumnsIfExists<Product>(["HasTierPrices", "HasDiscountsApplied"]);

        //#7242

        this.AddOrAlterColumnFor<Category>(t => t.RestrictFromVendors)
            .AsBoolean()
            .NotNullable()
            .WithDefaultValue(false);
        //#7281
        this.AddOrAlterColumnFor<Customer>(t => t.MustChangePassword)
            .AsBoolean()
            .NotNullable()
            .WithDefaultValue(false);

        //#7294
        var topicTableName = nameof(Topic);

        if (!Schema.Table(topicTableName).Index("IX_Topic_SystemName").Exists())
        {
            this.AddOrAlterColumnFor<Topic>(t => t.SystemName)
                .AsString(400)
                .Nullable();
        }

        //#7241
        this.AddOrAlterColumnFor<Discount>(t => t.VendorId)
            .AsInt32()
            .ForeignKey<Vendor>(onDelete: Rule.SetNull)
            .Nullable();

        //#7243
        this.AddOrAlterColumnFor<Vendor>(t => t.PmCustomerId)
        .AsInt32()
        .Nullable();
    }
}
