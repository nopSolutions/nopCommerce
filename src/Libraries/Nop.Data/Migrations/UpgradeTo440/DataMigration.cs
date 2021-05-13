using System.Linq;
using FluentMigrator;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Logging;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Vendors;
using Nop.Data.Mapping;

namespace Nop.Data.Migrations.UpgradeTo440
{
    [NopMigration("2020-06-10 00:00:00", "4.40.0", UpdateMigrationType.Data)]
    [SkipMigrationOnInstall]
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
            // new permission
            if (!_dataProvider.GetTable<PermissionRecord>().Any(pr => string.Compare(pr.SystemName, "AccessProfiling", true) == 0))
            {
                var profilingPermission = _dataProvider.InsertEntity(
                    new PermissionRecord
                    {
                        Name = "Public store. Access MiniProfiler results",
                        SystemName = "AccessProfiling",
                        Category = "PublicStore"
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
                        PermissionRecordId = profilingPermission.Id
                    }
                );
            }

            var activityLogTypeTable = _dataProvider.GetTable<ActivityLogType>();

            if (!activityLogTypeTable.Any(alt => string.Compare(alt.SystemKeyword, "AddNewSpecAttributeGroup", true) == 0))
                _dataProvider.InsertEntity(
                    new ActivityLogType
                    {
                        SystemKeyword = "AddNewSpecAttributeGroup",
                        Enabled = true,
                        Name = "Add a new specification attribute group"
                    }
                );

            if (!activityLogTypeTable.Any(alt => string.Compare(alt.SystemKeyword, "EditSpecAttributeGroup", true) == 0))
                _dataProvider.InsertEntity(
                    new ActivityLogType
                    {
                        SystemKeyword = "EditSpecAttributeGroup",
                        Enabled = true,
                        Name = "Edit a specification attribute group"
                    }
                );

            if (!activityLogTypeTable.Any(alt => string.Compare(alt.SystemKeyword, "DeleteSpecAttributeGroup", true) == 0))
                _dataProvider.InsertEntity(
                    new ActivityLogType
                    {
                        SystemKeyword = "DeleteSpecAttributeGroup",
                        Enabled = true,
                        Name = "Delete a specification attribute group"
                    }
                );
            //<MFA #475>
            if (!_dataProvider.GetTable<PermissionRecord>().Any(pr => string.Compare(pr.SystemName, "ManageMultifactorAuthenticationMethods", true) == 0))
            {
                var multiFactorAuthenticationPermission = _dataProvider.InsertEntity(
                    new PermissionRecord
                    {
                        Name = "Admin area. Manage Multi-factor Authentication Methods",
                        SystemName = "ManageMultifactorAuthenticationMethods",
                        Category = "Configuration"
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
                        PermissionRecordId = multiFactorAuthenticationPermission.Id
                    }
                );
            }
            //</MFA #475>

            //issue-3852
            var tableName = nameof(RewardPointsHistory);
            var rph = Schema.Table(tableName);
            var columnName = "UsedWithOrder_Id";

            if (rph.Column(columnName).Exists())
            {
                var constraintName = "RewardPointsHistory_UsedWithOrder";

                if (rph.Constraint(constraintName).Exists())
                    Delete.UniqueConstraint(constraintName).FromTable(tableName);

                Delete.Column(columnName).FromTable(tableName);
            }

            //#3353
            var productAttributeCombinationTableName = NameCompatibilityManager.GetTableName(typeof(ProductAttributeCombination));

            //add column
            if (!Schema.Table(productAttributeCombinationTableName).Column(nameof(ProductAttributeCombination.MinStockQuantity)).Exists())
            {
                Alter.Table(productAttributeCombinationTableName)
                    .AddColumn(nameof(ProductAttributeCombination.MinStockQuantity)).AsInt32().NotNullable().SetExistingRowsTo(0);
            }

            //#276 AJAX filters
            var categoryTableName = NameCompatibilityManager.GetTableName(typeof(Category));
            var manufacturerTableName = NameCompatibilityManager.GetTableName(typeof(Manufacturer));
            var vendorTableName = NameCompatibilityManager.GetTableName(typeof(Vendor));
            var productTableName = NameCompatibilityManager.GetTableName(typeof(Product));
            var orderTableName = NameCompatibilityManager.GetTableName(typeof(Order));
            var customerTableName = NameCompatibilityManager.GetTableName(typeof(Customer));

            //custom migrtion code
            var vegetarianColumnName = "Vegetarian";
            if (!Schema.Table(productTableName).Column(vegetarianColumnName).Exists())
            {
                Alter.Table(productTableName)
                    .AddColumn(vegetarianColumnName).AsBoolean().NotNullable().SetExistingRowsTo(false);
            }

            var veganColumnName = "Vegan";
            if (!Schema.Table(productTableName).Column(veganColumnName).Exists())
            {
                Alter.Table(productTableName)
                    .AddColumn(veganColumnName).AsBoolean().NotNullable().SetExistingRowsTo(false);
            }

            var gluttenFreeColumnName = "GluttenFree";
            if (!Schema.Table(productTableName).Column(gluttenFreeColumnName).Exists())
            {
                Alter.Table(productTableName)
                    .AddColumn(gluttenFreeColumnName).AsBoolean().NotNullable().SetExistingRowsTo(false);
            }

            var halalColumnName = "Halal";
            if (!Schema.Table(productTableName).Column(halalColumnName).Exists())
            {
                Alter.Table(productTableName)
                    .AddColumn(halalColumnName).AsBoolean().NotNullable().SetExistingRowsTo(false);
            }

            var allergyFriendlyColumnName = "AllergyFriendly";
            if (!Schema.Table(productTableName).Column(allergyFriendlyColumnName).Exists())
            {
                Alter.Table(productTableName)
                    .AddColumn(allergyFriendlyColumnName).AsBoolean().NotNullable().SetExistingRowsTo(false);
            }

            var wellPackedColumnName = "WellPacked";
            if (!Schema.Table(productTableName).Column(wellPackedColumnName).Exists())
            {
                Alter.Table(productTableName)
                    .AddColumn(wellPackedColumnName).AsBoolean().NotNullable().SetExistingRowsTo(false);
            }

            var sustainablePackagingColumnName = "SustainablePackaging";
            if (!Schema.Table(productTableName).Column(sustainablePackagingColumnName).Exists())
            {
                Alter.Table(productTableName)
                    .AddColumn(sustainablePackagingColumnName).AsBoolean().NotNullable().SetExistingRowsTo(false);
            }

            var fastAndReliableColumnName = "FastAndReliable";
            if (!Schema.Table(productTableName).Column(fastAndReliableColumnName).Exists())
            {
                Alter.Table(productTableName)
                    .AddColumn(fastAndReliableColumnName).AsBoolean().NotNullable().SetExistingRowsTo(false);
            }

            var excellentValueColumnName = "ExcellentValue";
            if (!Schema.Table(productTableName).Column(excellentValueColumnName).Exists())
            {
                Alter.Table(productTableName)
                    .AddColumn(excellentValueColumnName).AsBoolean().NotNullable().SetExistingRowsTo(false);
            }

            var followOrderNotesColumnName = "FollowOrderNotes";
            if (!Schema.Table(productTableName).Column(followOrderNotesColumnName).Exists())
            {
                Alter.Table(productTableName)
                    .AddColumn(followOrderNotesColumnName).AsBoolean().NotNullable().SetExistingRowsTo(false);
            }

            var minimumColumnName = "Minimum";
            if (!Schema.Table(productTableName).Column(minimumColumnName).Exists())
            {
                Alter.Table(productTableName)
                    .AddColumn(minimumColumnName).AsBoolean().NotNullable().SetExistingRowsTo(false);
            }

            var averageColumnName = "Average";
            if (!Schema.Table(productTableName).Column(averageColumnName).Exists())
            {
                Alter.Table(productTableName)
                    .AddColumn(averageColumnName).AsBoolean().NotNullable().SetExistingRowsTo(false);
            }

            var expensiveColumnName = "Expensive";
            if (!Schema.Table(productTableName).Column(expensiveColumnName).Exists())
            {
                Alter.Table(productTableName)
                    .AddColumn(expensiveColumnName).AsBoolean().NotNullable().SetExistingRowsTo(false);
            }


            var ribbonEnableColumnName = "RibbonEnable";
            if (!Schema.Table(productTableName).Column(ribbonEnableColumnName).Exists())
            {
                Alter.Table(productTableName)
                    .AddColumn(ribbonEnableColumnName).AsBoolean().NotNullable().SetExistingRowsTo(false);
            }

            var ribbonTextColumnName = "RibbonText";
            if (!Schema.Table(productTableName).Column(ribbonTextColumnName).Exists())
            {
                Alter.Table(productTableName)
                    .AddColumn(ribbonTextColumnName).AsString().Nullable().SetExistingRowsTo(null);
            }

            //order
            var scheduleDateColumnName = "ScheduleDate";
            if (!Schema.Table(orderTableName).Column(scheduleDateColumnName).Exists())
            {
                Alter.Table(orderTableName)
                    .AddColumn(scheduleDateColumnName).AsString().Nullable().SetExistingRowsTo(null);
            }

            //customer
            var pushTokenColumnName = "PushToken";
            if (!Schema.Table(customerTableName).Column(pushTokenColumnName).Exists())
            {
                Alter.Table(customerTableName)
                    .AddColumn(pushTokenColumnName).AsString().Nullable().SetExistingRowsTo(null);
            }
            var offersColumnName = "Offers";
            if (!Schema.Table(customerTableName).Column(offersColumnName).Exists())
            {
                Alter.Table(customerTableName)
                    .AddColumn(offersColumnName).AsBoolean().NotNullable().SetExistingRowsTo(false);
            }
            var rewardsColumnName = "Rewards";
            if (!Schema.Table(customerTableName).Column(rewardsColumnName).Exists())
            {
                Alter.Table(customerTableName)
                    .AddColumn(rewardsColumnName).AsBoolean().NotNullable().SetExistingRowsTo(false);
            }
            var eatsPassColumnName = "EatsPass";
            if (!Schema.Table(customerTableName).Column(eatsPassColumnName).Exists())
            {
                Alter.Table(customerTableName)
                    .AddColumn(eatsPassColumnName).AsBoolean().NotNullable().SetExistingRowsTo(false);
            }
            var otherColumnName = "Other";
            if (!Schema.Table(customerTableName).Column(otherColumnName).Exists())
            {
                Alter.Table(customerTableName)
                    .AddColumn(otherColumnName).AsBoolean().NotNullable().SetExistingRowsTo(false);
            }

            //remove column
            var priceRangesColumnName = "PriceRanges";

            if (Schema.Table(categoryTableName).Column(priceRangesColumnName).Exists())
                Delete.Column(priceRangesColumnName).FromTable(categoryTableName);

            if (Schema.Table(manufacturerTableName).Column(priceRangesColumnName).Exists())
                Delete.Column(priceRangesColumnName).FromTable(manufacturerTableName);

            //add column
            var priceRangeFilteringColumnName = "PriceRangeFiltering";

            if (!Schema.Table(categoryTableName).Column(priceRangeFilteringColumnName).Exists())
            {
                Alter.Table(categoryTableName)
                    .AddColumn(priceRangeFilteringColumnName).AsBoolean().NotNullable().SetExistingRowsTo(true);
            }
            
            if (!Schema.Table(manufacturerTableName).Column(priceRangeFilteringColumnName).Exists())
            {
                Alter.Table(manufacturerTableName)
                    .AddColumn(priceRangeFilteringColumnName).AsBoolean().NotNullable().SetExistingRowsTo(true);
            }

            if (!Schema.Table(vendorTableName).Column(priceRangeFilteringColumnName).Exists())
            {
                Alter.Table(vendorTableName)
                    .AddColumn(priceRangeFilteringColumnName).AsBoolean().NotNullable().SetExistingRowsTo(true);
            }

            //add column
            var priceFromColumnName = "PriceFrom";

            if (!Schema.Table(categoryTableName).Column(priceFromColumnName).Exists())
            {
                Alter.Table(categoryTableName)
                    .AddColumn(priceFromColumnName).AsDecimal().NotNullable().SetExistingRowsTo(0);
            }

            if (!Schema.Table(manufacturerTableName).Column(priceFromColumnName).Exists())
            {
                Alter.Table(manufacturerTableName)
                    .AddColumn(priceFromColumnName).AsDecimal().NotNullable().SetExistingRowsTo(0);
            }

            if (!Schema.Table(vendorTableName).Column(priceFromColumnName).Exists())
            {
                Alter.Table(vendorTableName)
                    .AddColumn(priceFromColumnName).AsDecimal().NotNullable().SetExistingRowsTo(0);
            }

            //add column
            var priceToColumnName = "PriceTo";

            if (!Schema.Table(categoryTableName).Column(priceToColumnName).Exists())
            {
                Alter.Table(categoryTableName)
                    .AddColumn(priceToColumnName).AsDecimal().NotNullable().SetExistingRowsTo(10000);
            }

            if (!Schema.Table(manufacturerTableName).Column(priceToColumnName).Exists())
            {
                Alter.Table(manufacturerTableName)
                    .AddColumn(priceToColumnName).AsDecimal().NotNullable().SetExistingRowsTo(10000);
            }

            if (!Schema.Table(vendorTableName).Column(priceToColumnName).Exists())
            {
                Alter.Table(vendorTableName)
                    .AddColumn(priceToColumnName).AsDecimal().NotNullable().SetExistingRowsTo(10000);
            }

            //add column
            var manuallyPriceRangeColumnName = "ManuallyPriceRange";

            if (!Schema.Table(categoryTableName).Column(manuallyPriceRangeColumnName).Exists())
            {
                Alter.Table(categoryTableName)
                    .AddColumn(manuallyPriceRangeColumnName).AsBoolean().NotNullable().SetExistingRowsTo(false);
            }

            if (!Schema.Table(manufacturerTableName).Column(manuallyPriceRangeColumnName).Exists())
            {
                Alter.Table(manufacturerTableName)
                    .AddColumn(manuallyPriceRangeColumnName).AsBoolean().NotNullable().SetExistingRowsTo(false);
            }

            if (!Schema.Table(vendorTableName).Column(manuallyPriceRangeColumnName).Exists())
            {
                Alter.Table(vendorTableName)
                    .AddColumn(manuallyPriceRangeColumnName).AsBoolean().NotNullable().SetExistingRowsTo(false);
            }
        }

        public override void Down()
        {
            //add the downgrade logic if necessary 
        }
    }
}
