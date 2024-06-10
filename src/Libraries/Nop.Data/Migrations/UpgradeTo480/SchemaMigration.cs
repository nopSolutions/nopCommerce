using FluentMigrator;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Forums;
using Nop.Core.Domain.Logging;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.Orders;
using Nop.Data.Extensions;
using Nop.Data.Mapping;

namespace Nop.Data.Migrations.UpgradeTo480;

[NopSchemaMigration("2024-06-10 00:00:00", "SchemaMigration for 4.80.0")]
public class SchemaMigration : ForwardOnlyMigration
{
    /// <summary>
    /// Collect the UP migration expressions
    /// </summary>
    public override void Up()
    {
        //#7188
        var ptoductTableName = nameof(Product);
        var hasDiscountsAppliedColumnName = "HasDiscountsApplied";
        if (Schema.Table(ptoductTableName).Column(hasDiscountsAppliedColumnName).Exists())
            Delete.Column(hasDiscountsAppliedColumnName).FromTable(ptoductTableName);
    }
}