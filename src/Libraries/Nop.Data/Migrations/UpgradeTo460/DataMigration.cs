using System;
using System.Linq;
using FluentMigrator;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.ScheduleTasks;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Shipping;
using Nop.Data.Mapping;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Logging;

namespace Nop.Data.Migrations.UpgradeTo460
{
    [NopMigration("2022-02-10 00:00:00", "4.60.0", UpdateMigrationType.Data, MigrationProcessType.Update)]
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

            if (!activityLogTypeTable.Any(alt => string.Compare(alt.SystemKeyword, "ImportOrders", StringComparison.InvariantCultureIgnoreCase) == 0))
                _dataProvider.InsertEntity(
                    new ActivityLogType
                    {
                        SystemKeyword = "ImportOrders",
                        Enabled = true,
                        Name = "Orders were imported"
                    }
                );

            if (!activityLogTypeTable.Any(alt => string.Compare(alt.SystemKeyword, "ImportNewsLetterSubscriptions", StringComparison.InvariantCultureIgnoreCase) == 0))
                _dataProvider.InsertEntity(
                    new ActivityLogType
                    {
                        SystemKeyword = "ImportNewsLetterSubscriptions",
                        Enabled = true,
                        Name = "Newsletter subscriptions were imported"
                    }
                );

            if (!activityLogTypeTable.Any(alt => string.Compare(alt.SystemKeyword, "ExportCustomers", StringComparison.InvariantCultureIgnoreCase) == 0))
                _dataProvider.InsertEntity(
                    new ActivityLogType
                    {
                        SystemKeyword = "ExportCustomers",
                        Enabled = true,
                        Name = "Customers were exported"
                    }
                );

            if (!activityLogTypeTable.Any(alt => string.Compare(alt.SystemKeyword, "ExportCategories", StringComparison.InvariantCultureIgnoreCase) == 0))
                _dataProvider.InsertEntity(
                    new ActivityLogType
                    {
                        SystemKeyword = "ExportCategories",
                        Enabled = true,
                        Name = "Categories were exported"
                    }
                );

            if (!activityLogTypeTable.Any(alt => string.Compare(alt.SystemKeyword, "ExportManufacturers", StringComparison.InvariantCultureIgnoreCase) == 0))
                _dataProvider.InsertEntity(
                    new ActivityLogType
                    {
                        SystemKeyword = "ExportManufacturers",
                        Enabled = true,
                        Name = "Manufacturers were exported"
                    }
                );

            if (!activityLogTypeTable.Any(alt => string.Compare(alt.SystemKeyword, "ExportProducts", StringComparison.InvariantCultureIgnoreCase) == 0))
                _dataProvider.InsertEntity(
                    new ActivityLogType
                    {
                        SystemKeyword = "ExportProducts",
                        Enabled = true,
                        Name = "Products were exported"
                    }
                );

            if (!activityLogTypeTable.Any(alt => string.Compare(alt.SystemKeyword, "ExportOrders", StringComparison.InvariantCultureIgnoreCase) == 0))
                _dataProvider.InsertEntity(
                    new ActivityLogType
                    {
                        SystemKeyword = "ExportOrders",
                        Enabled = true,
                        Name = "Orders were exported"
                    }
                );

            if (!activityLogTypeTable.Any(alt => string.Compare(alt.SystemKeyword, "ExportStates", StringComparison.InvariantCultureIgnoreCase) == 0))
                _dataProvider.InsertEntity(
                    new ActivityLogType
                    {
                        SystemKeyword = "ExportStates",
                        Enabled = true,
                        Name = "States were exported"
                    }
                );

            if (!activityLogTypeTable.Any(alt => string.Compare(alt.SystemKeyword, "ExportNewsLetterSubscriptions", StringComparison.InvariantCultureIgnoreCase) == 0))
                _dataProvider.InsertEntity(
                    new ActivityLogType
                    {
                        SystemKeyword = "ExportNewsLetterSubscriptions",
                        Enabled = true,
                        Name = "Newsletter subscriptions were exported"
                    }
                );
        }

        public override void Down()
        {
            //add the downgrade logic if necessary 
        }
    }
}
