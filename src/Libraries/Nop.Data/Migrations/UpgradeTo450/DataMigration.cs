using FluentMigrator;
using Nop.Core.Domain.ScheduleTasks;
using Nop.Data.Mapping;
using Nop.Core.Domain.Orders;

namespace Nop.Data.Migrations.UpgradeTo450
{
    [NopMigration("2021-04-23 00:00:00", "4.50.0", UpdateMigrationType.Data, MigrationProcessType.Update)]
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
            //#5547
            var scheduleTaskTableName = NameCompatibilityManager.GetTableName(typeof(ScheduleTask));

            //add column
            if (!Schema.Table(scheduleTaskTableName).Column(nameof(ScheduleTask.LastEnabledUtc)).Exists())
            {
                Alter.Table(scheduleTaskTableName)
                    .AddColumn(nameof(ScheduleTask.LastEnabledUtc)).AsDateTime().Nullable();
            }

            //add column
            var returnRequestTableName = NameCompatibilityManager.GetTableName(typeof(ReturnRequest));
            var returnedQuantityColumnName = "ReturnedQuantity";

            if (!Schema.Table(returnRequestTableName).Column(returnedQuantityColumnName).Exists())
            {
                Alter.Table(returnRequestTableName)
                    .AddColumn(returnedQuantityColumnName).AsInt32().NotNullable().SetExistingRowsTo(0);
            }
        }

        public override void Down()
        {
            //add the downgrade logic if necessary 
        }
    }
}
