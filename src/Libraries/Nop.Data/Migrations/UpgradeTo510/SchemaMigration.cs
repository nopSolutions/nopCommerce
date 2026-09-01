using System.Data;
using FluentMigrator;
using Nop.Core.Domain.Affiliates;
using Nop.Core.Domain.Blogs;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.UpgradeTo510;

[NopSchemaMigration("2026-11-11 00:00:01", "SchemaMigration for 5.10.0")]
public class SchemaMigration : ForwardOnlyMigration
{
    /// <summary>
    /// Collect the UP migration expressions
    /// </summary>
    public override void Up()
    {
        //#8224
        this.AddOrAlterForeignKeyColumnFor<BlogComment, Customer>(record => record.CustomerId, "BlogComment_Customer")
            .OnDelete(Rule.SetNull)
            .Nullable();
        this.AddOrAlterForeignKeyColumnFor<ProductReview, Customer>(record => record.CustomerId, "ProductReview_Customer")
            .OnDelete(Rule.SetNull)
            .Nullable();
        this.AddOrAlterForeignKeyColumnFor<SearchTerm, Customer>(record => record.CustomerId)
            .OnDelete(Rule.SetNull)
            .Nullable();

        //#8273
        this.AddOrAlterForeignKeyColumnFor<Affiliate, Customer>(a => a.AssociatedCustomerId)
            .OnDelete(Rule.SetNull)
            .Nullable();
        this.AddOrAlterColumnFor<Affiliate>(a => a.CreatedOnUtc).AsDateTime2().Nullable();

        this.AddOrAlterColumnFor<Product>(p => p.SpecifyAffiliateCommission)
            .AsBoolean()
            .NotNullable()
            .SetExistingRowsTo(false);
        this.AddOrAlterColumnFor<Product>(p => p.AffiliateCommissionAmount)
            .AsDecimal(18, 4)
            .Nullable();
        this.AddOrAlterColumnFor<Product>(p => p.AffiliateCommissionPercentage)
            .AsDecimal(18, 4)
            .Nullable();
        this.AddOrAlterColumnFor<Product>(p => p.AffiliateUsePercentage)
            .AsBoolean()
            .NotNullable()
            .SetExistingRowsTo(false);

        this.CreateTableIfNotExists<AffiliateCommission>();

        this.AddOrAlterColumnFor<Order>(o => o.AffiliateCommissionAmount)
            .AsDecimal(18, 4)
            .Nullable();
        this.AddOrAlterColumnFor<Order>(o => o.AffiliateCommissionId)
            .AsInt32()
            .Nullable();
    }
}