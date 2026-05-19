using FluentMigrator;
using Nop.Core.Domain.ScheduleTasks;

namespace Nop.Data.Migrations.UpgradeTo500;

[NopUpdateMigration("2026-05-19 00:00:00", "5.00", UpdateMigrationType.Data)]
public class OutboxPublisherTaskMigration : Migration
{
    protected readonly INopDataProvider _dataProvider;

    private const string OldType = "Nop.Services.Integration.SpikeOutboxPublisherTask, Nop.Services";
    private const string NewType = "Nop.Services.Integration.OutboxPublisherTask, Nop.Services";
    private const string TaskName = "Outbox publisher (RabbitMQ)";

    public OutboxPublisherTaskMigration(INopDataProvider dataProvider)
    {
        _dataProvider = dataProvider;
    }

    public override void Up()
    {
        var tasks = _dataProvider.GetTable<ScheduleTask>();

        var existing = tasks.FirstOrDefault(t => t.Type == OldType || t.Type == NewType);

        if (existing != null)
        {
            if (existing.Type != NewType)
            {
                existing.Type = NewType;
                existing.Name = TaskName;
                _dataProvider.UpdateEntity(existing);
            }
            return;
        }

        _dataProvider.InsertEntity(new ScheduleTask
        {
            Name = TaskName,
            Seconds = 10,
            Type = NewType,
            Enabled = true,
            StopOnError = false
        });
    }

    public override void Down()
    {
    }
}
