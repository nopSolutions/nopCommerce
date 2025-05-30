using FluentMigrator;
using Nop.Core.Domain.Topics;

namespace Nop.Data.Migrations.UpgradeTo490;

[NopSchemaMigration("2025-01-28 00:00:00", "AddIndexesMigration for 4.90.0")]
public class AddIndexesMigration : ForwardOnlyMigration
{
    private readonly INopDataProvider _dataProvider;

    public AddIndexesMigration(INopDataProvider dataProvider)
    {
        _dataProvider = dataProvider;
    }

    /// <summary>
    /// Collect the UP migration expressions
    /// </summary>
    public override void Up()
    {
        
        //#7296
        var topicTableName = nameof(Topic);
        var topicEndDateColumnName = nameof(Topic.AvailableEndDateTimeUtc);
        var topicStartDateColumnName = nameof(Topic.AvailableStartDateTimeUtc);
        var topicAvailableDatesIndexName = "IX_Topic_Availability";
        if (!Schema.Table(topicTableName).Index(topicAvailableDatesIndexName).Exists())
            Create.Index(topicAvailableDatesIndexName)
                .OnTable(topicTableName)
                .OnColumn(topicEndDateColumnName).Descending()
                .OnColumn(topicStartDateColumnName).Descending()
                .WithOptions().NonClustered();
    }
}