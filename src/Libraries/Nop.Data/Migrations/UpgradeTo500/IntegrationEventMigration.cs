using FluentMigrator;
using Nop.Core.Domain.Events;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.UpgradeTo500;

[NopSchemaMigration("2026-05-04 00:00:01", "IntegrationEvent spike migration")]
public class IntegrationEventMigration : ForwardOnlyMigration
{
    /// <summary>
    /// Collect the UP migration expressions
    /// </summary>
    public override void Up()
    {
        this.CreateTableIfNotExists<IntegrationEvent>();
    }
}
