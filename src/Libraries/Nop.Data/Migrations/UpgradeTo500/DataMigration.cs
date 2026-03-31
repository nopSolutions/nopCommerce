using FluentMigrator;
using Nop.Core.Domain.ScheduleTasks;

namespace Nop.Data.Migrations.UpgradeTo500;

[NopUpdateMigration("2026-03-31 00:00:00", "5.00", UpdateMigrationType.Data)]
public class DataMigration : Migration
{
    private readonly INopDataProvider _dataProvider;

    public DataMigration(INopDataProvider dataProvider)
    {
        _dataProvider = dataProvider;
    }

    /// <summary>
    /// Collect the UP migration expressions
    /// </summary>
    public override void Up()
    {
        //#8120
        if (!_dataProvider.GetTable<ScheduleTask>().Any(st => string.Compare(st.Type, "Nop.Services.Orders.AutoCancelPaidOrdersTask, Nop.Services", StringComparison.InvariantCultureIgnoreCase) == 0))
        {
            _dataProvider.InsertEntity(new ScheduleTask()
            {
                Name = "Auto-cancel unpaid orders",
                Seconds = 600,
                Type = "Nop.Services.Orders.AutoCancelPaidOrdersTask, Nop.Services",
                Enabled = true,
                LastEnabledUtc = DateTime.UtcNow,
                StopOnError = false
            });
        }
    }

    public override void Down()
    {
        //add the downgrade logic if necessary 
    }
}