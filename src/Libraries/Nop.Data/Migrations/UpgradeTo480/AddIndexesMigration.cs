using FluentMigrator;
using Nop.Core.Domain.Customers;

namespace Nop.Data.Migrations.UpgradeTo480;

[NopSchemaMigration("2024-06-21 19:53:00", "AddIndexesMigration for 4.80.0")]
public class AddIndexesMigration : ForwardOnlyMigration
{
    /// <summary>
    /// Collect the UP migration expressions
    /// </summary>
    public override void Up()
    {
        if (!Schema.Table(nameof(Customer)).Index("IX_Customer_Deleted").Exists())
            Create.Index("IX_Customer_Deleted")
                .OnTable(nameof(Customer))
                .OnColumn(nameof(Customer.Deleted)).Ascending()
                .WithOptions().NonClustered();
    }
}