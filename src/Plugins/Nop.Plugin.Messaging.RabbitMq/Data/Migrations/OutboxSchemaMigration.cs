using FluentMigrator;
using Nop.Data.Extensions;
using Nop.Data.Migrations;
using Nop.Plugin.Messaging.RabbitMq.Domain;

namespace Nop.Plugin.Messaging.RabbitMq.Data.Migrations;

[NopMigration("2026/05/12 10:00:00", "Nop.Plugin.Messaging.RabbitMq outbox schema", MigrationProcessType.Installation)]
public class OutboxSchemaMigration : Migration
{
    public override void Up()
    {
        this.CreateTableIfNotExists<OutboxMessage>();
    }

    public override void Down()
    {
        this.DeleteTableIfExists<OutboxMessage>();
    }
}
