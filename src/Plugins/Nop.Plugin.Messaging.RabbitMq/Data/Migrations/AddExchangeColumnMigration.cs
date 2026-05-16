using FluentMigrator;
using Nop.Data.Migrations;

namespace Nop.Plugin.Messaging.RabbitMq.Data.Migrations;

[NopMigration("2026/05/16 19:50:00", "Nop.Plugin.Messaging.RabbitMq add Exchange column to OutboxMessage", MigrationProcessType.Update)]
public class AddExchangeColumnMigration : Migration
{
    public override void Up()
    {
        if (!Schema.Table("OutboxMessage").Column("Exchange").Exists())
            Alter.Table("OutboxMessage")
                .AddColumn("Exchange").AsString(200).Nullable();
    }

    public override void Down()
    {
        if (Schema.Table("OutboxMessage").Column("Exchange").Exists())
            Delete.Column("Exchange").FromTable("OutboxMessage");
    }
}
