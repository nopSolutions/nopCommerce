using System;
using System.Linq;
using FluentMigrator;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.ScheduleTasks;
using Nop.Core.Domain.Security;
using Nop.Data.Mapping;

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
                    .AddColumn(nameof(ScheduleTask.LastEnabledUtc)).AsDateTime2().Nullable();
            }
            else
            {
                Alter.Table(scheduleTaskTableName).AlterColumn(nameof(ScheduleTask.LastEnabledUtc)).AsDateTime2().Nullable();
            }

            //#5939
            if (!_dataProvider.GetTable<PermissionRecord>().Any(pr => string.Compare(pr.SystemName, "SalesSummaryReport", StringComparison.InvariantCultureIgnoreCase) == 0))
            {
                var salesSummaryReportPermission = _dataProvider.InsertEntity(
                    new PermissionRecord
                    {
                        Name = "Admin area. Access sales summary report",
                        SystemName = "SalesSummaryReport",
                        Category = "Orders"
                    }
                );

                //add it to the Admin role by default
                var adminRole = _dataProvider
                    .GetTable<CustomerRole>()
                    .FirstOrDefault(x => x.IsSystemRole && x.SystemName == NopCustomerDefaults.AdministratorsRoleName);

                _dataProvider.InsertEntity(
                    new PermissionRecordCustomerRoleMapping
                    {
                        CustomerRoleId = adminRole.Id,
                        PermissionRecordId = salesSummaryReportPermission.Id
                    }
                );
            }
        }

        public override void Down()
        {
            //add the downgrade logic if necessary 
        }
    }
}
