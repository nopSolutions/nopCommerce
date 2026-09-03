using System.Data;
using FluentMigrator;
using Nop.Core.Domain.Blogs;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.UpgradeTo510;

[NopSchemaMigration("2026-11-11 00:00:00", "SchemaMigration for 5.10.0")]
public class SchemaMigration : ForwardOnlyMigration
{
    /// <summary>
    /// Collect the UP migration expressions
    /// </summary>
    public override void Up()
    {
        //#8224
        this.AddOrAlterForeignKeyColumnFor<BlogComment, Customer>(t => t.CustomerId, onDelete: Rule.SetNull).Nullable();
        this.AddOrAlterForeignKeyColumnFor<ProductReview, Customer>(t => t.CustomerId, onDelete: Rule.SetNull).Nullable();
        this.AddOrAlterForeignKeyColumnFor<SearchTerm, Customer>(t => t.CustomerId, onDelete: Rule.SetNull).Nullable();
    }
}