using FluentMigrator;
using Nop.Core.Domain.Customers;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.UpgradeTo500;

[NopSchemaMigration("2026-01-22 00:00:10", "SchemaMigration for 5.00.0")]
public class SchemaMigration : ForwardOnlyMigration
{
    /// <summary>
    /// Collect the UP migration expressions
    /// </summary>
    public override void Up()
    {
        //#2430
        this.AddOrAlterColumnFor<Customer>(c => c.PhoneSmsVerified)
            .AsBoolean()
            .NotNullable()
            .SetExistingRowsTo(false);
    }
}
