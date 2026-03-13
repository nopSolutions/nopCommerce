using FluentMigrator;
using Nop.Data.Extensions;
using Nop.Data.Migrations;
using Nop.Plugin.Payments.Paystack.Models;

namespace Nop.Plugin.Payments.Paystack.Data;

[NopMigration("2025/02/09 12:00:00:0000000", "Paystack base schema", MigrationProcessType.Installation)]
public class SchemaMigration : AutoReversingMigration
{
    /// <inheritdoc />
    public override void Up()
    {
        this.CreateTableIfNotExists<PaystackTransactionModel>();
    }
}
