using FluentMigrator;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Logging;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Vendors;
using Nop.Data.Extensions;
using Nop.Data.Mapping;

namespace Nop.Data.Migrations.UpgradeTo440;

[NopUpdateMigration("2020-06-10 00:00:00", "4.40", UpdateMigrationType.Data)]
public class DataMigration : Migration
{
    protected readonly INopDataProvider _dataProvider;

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
        if (!_dataProvider.GetTable<PermissionRecord>().Any(pr => string.Compare(pr.SystemName, "AccessProfiling", StringComparison.InvariantCultureIgnoreCase) == 0))
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

        if (!activityLogTypeTable.Any(alt => string.Compare(alt.SystemKeyword, "AddNewSpecAttributeGroup", StringComparison.InvariantCultureIgnoreCase) == 0))
            _dataProvider.InsertEntity(
                new ActivityLogType
                {
                    SystemKeyword = "AddNewSpecAttributeGroup",
                    Enabled = true,
                    Name = "Add a new specification attribute group"
                }
            );

        if (!activityLogTypeTable.Any(alt => string.Compare(alt.SystemKeyword, "EditSpecAttributeGroup", StringComparison.InvariantCultureIgnoreCase) == 0))
            _dataProvider.InsertEntity(
                new ActivityLogType
                {
                    SystemKeyword = "EditSpecAttributeGroup",
                    Enabled = true,
                    Name = "Edit a specification attribute group"
                }
            );

        if (!activityLogTypeTable.Any(alt => string.Compare(alt.SystemKeyword, "DeleteSpecAttributeGroup", StringComparison.InvariantCultureIgnoreCase) == 0))
            _dataProvider.InsertEntity(
                new ActivityLogType
                {
                    SystemKeyword = "DeleteSpecAttributeGroup",
                    Enabled = true,
                    Name = "Delete a specification attribute group"
                }
            );
        //<MFA #475>
        if (!_dataProvider.GetTable<PermissionRecord>().Any(pr => string.Compare(pr.SystemName, "ManageMultifactorAuthenticationMethods", StringComparison.InvariantCultureIgnoreCase) == 0))
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
        var rewardPointsHistoryTableName = NameCompatibilityManager.GetTableName(typeof(RewardPointsHistory));
        var rph = Schema.Table(rewardPointsHistoryTableName);
        var columnName = "UsedWithOrder_Id";

        if (Schema.ColumnExist<RewardPointsHistory>(columnName))
        {
            var constraintName = "RewardPointsHistory_UsedWithOrder";

            if (rph.Constraint(constraintName).Exists())
                Delete.UniqueConstraint(constraintName).FromTable(rewardPointsHistoryTableName);

            Delete.Column<RewardPointsHistory>(columnName);
        }

        //#3353

        //add column
        if (!Schema.ColumnExist<ProductAttributeCombination>(t => t.MinStockQuantity))
        {
            Alter.AddColumnFor<ProductAttributeCombination>(t => t.MinStockQuantity)
                .AsInt32()
                .NotNullable()
                .SetExistingRowsTo(0);
        }

        //#276 AJAX filters

        //remove column
        var priceRangesColumnName = "PriceRanges";

        if (Schema.ColumnExist<Category>(priceRangesColumnName))
            Delete.Column<Category>(priceRangesColumnName);

        if (Schema.ColumnExist<Manufacturer>(priceRangesColumnName))
            Delete.Column<Manufacturer>(priceRangesColumnName);

        //add column
        if (!Schema.ColumnExist<Category>(t => t.PriceRangeFiltering))
        {
            Alter.AddColumnFor<Category>(t => t.PriceRangeFiltering)
                .AsBoolean()
                .NotNullable()
                .SetExistingRowsTo(true);
        }

        if (!Schema.ColumnExist<Manufacturer>(t => t.PriceRangeFiltering))
        {
            Alter.AddColumnFor<Manufacturer>(t => t.PriceRangeFiltering)
                .AsBoolean()
                .NotNullable()
                .SetExistingRowsTo(true);
        }

        if (!Schema.ColumnExist<Vendor>(t => t.PriceRangeFiltering))
        {
            Alter.AddColumnFor<Vendor>(t => t.PriceRangeFiltering)
                .AsBoolean()
                .NotNullable()
                .SetExistingRowsTo(true);
        }

        //add column
        if (!Schema.ColumnExist<Category>(t => t.PriceFrom))
        {
            Alter.AddColumnFor<Category>(t => t.PriceFrom)
                .AsDecimal()
                .NotNullable()
                .SetExistingRowsTo(0);
        }

        if (!Schema.ColumnExist<Manufacturer>(t => t.PriceFrom))
        {
            Alter.AddColumnFor<Manufacturer>(t => t.PriceFrom)
                .AsDecimal()
                .NotNullable()
                .SetExistingRowsTo(0);
        }

        if (!Schema.ColumnExist<Vendor>(t => t.PriceFrom))
        {
            Alter.AddColumnFor<Vendor>(t => t.PriceFrom)
                .AsDecimal()
                .NotNullable()
                .SetExistingRowsTo(0);
        }

        //add column
        if (!Schema.ColumnExist<Category>(t => t.PriceTo))
        {
            Alter.AddColumnFor<Category>(t => t.PriceTo)
                .AsDecimal()
                .NotNullable()
                .SetExistingRowsTo(10000);
        }

        if (!Schema.ColumnExist<Manufacturer>(t => t.PriceTo))
        {
            Alter.AddColumnFor<Manufacturer>(t => t.PriceTo)
                .AsDecimal()
                .NotNullable()
                .SetExistingRowsTo(10000);
        }

        if (!Schema.ColumnExist<Vendor>(t => t.PriceTo))
        {
            Alter.AddColumnFor<Vendor>(t => t.PriceTo)
                .AsDecimal()
                .NotNullable()
                .SetExistingRowsTo(10000);
        }

        //add column
        if (!Schema.ColumnExist<Category>(t => t.ManuallyPriceRange))
        {
            Alter.AddColumnFor<Category>(t => t.ManuallyPriceRange)
                .AsBoolean()
                .NotNullable()
                .SetExistingRowsTo(false);
        }

        if (!Schema.ColumnExist<Manufacturer>(t => t.ManuallyPriceRange))
        {
            Alter.AddColumnFor<Manufacturer>(t => t.ManuallyPriceRange)
                .AsBoolean()
                .NotNullable()
                .SetExistingRowsTo(false);
        }

        if (!Schema.ColumnExist<Vendor>(t => t.ManuallyPriceRange))
        {
            Alter.AddColumnFor<Vendor>(t => t.ManuallyPriceRange)
                .AsBoolean()
                .NotNullable()
                .SetExistingRowsTo(false);
        }
    }

    public override void Down()
    {
        //add the downgrade logic if necessary 
    }
}