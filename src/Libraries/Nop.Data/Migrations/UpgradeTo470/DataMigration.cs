using FluentMigrator;
using Nop.Core.Domain.Logging;

namespace Nop.Data.Migrations.UpgradeTo470
{
    [NopMigration("2023-01-01 00:00:00", "4.70.0", UpdateMigrationType.Data, MigrationProcessType.Update)]
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
            //#5312 new activity log type
            var activityLogTypeTable = _dataProvider.GetTable<ActivityLogType>();

            if (!activityLogTypeTable.Any(alt => string.Compare(alt.SystemKeyword, "ImportCustomers", StringComparison.InvariantCultureIgnoreCase) == 0))
                _dataProvider.InsertEntity(
                    new ActivityLogType
                    {
                        SystemKeyword = "ImportCustomers",
                        Enabled = true,
                        Name = "Customers were imported"
                    }
                );
            //6660 new activity log type for update plugin
            if (!activityLogTypeTable.Any(alt => string.Compare(alt.SystemKeyword, "UpdatePlugin", StringComparison.InvariantCultureIgnoreCase) == 0))
                _dataProvider.InsertEntity(
                    new ActivityLogType
                    {
                        SystemKeyword = "UpdatePlugin",
                        Enabled = true,
                        Name = "Update a plugin"
                    }
                );
        }

        public override void Down()
        {
            //add the downgrade logic if necessary 
        }
    }
}
