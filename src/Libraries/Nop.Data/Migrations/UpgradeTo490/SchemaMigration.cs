using FluentMigrator;
using Nop.Core.Domain.Topics;

namespace Nop.Data.Migrations.UpgradeTo490;

[NopSchemaMigration("2025-01-27 00:00:00", "SchemaMigration for 4.90.0")]
public class SchemaMigration : ForwardOnlyMigration
{
    /// <summary>
    /// Collect the UP migration expressions
    /// </summary>
    public override void Up()
    {
        //#7294
        var topicTableName = nameof(Topic);
        var topicAvailableEndDateColumnName = nameof(Topic.AvailableEndDateTimeUtc);
        var topicAvailableStartDateColumnName = nameof(Topic.AvailableStartDateTimeUtc);

        if (!Schema.Table(topicTableName).Column(topicAvailableEndDateColumnName).Exists())
        {
            Alter.Table(topicTableName)
                .AddColumn(topicAvailableEndDateColumnName)
                .AsDateTime()
                .Nullable();
        }

        if (!Schema.Table(topicTableName).Column(topicAvailableStartDateColumnName).Exists())
        {
            Alter.Table(topicTableName)
                .AddColumn(topicAvailableStartDateColumnName)
                .AsDateTime()
                .Nullable();
        }
    }
}
