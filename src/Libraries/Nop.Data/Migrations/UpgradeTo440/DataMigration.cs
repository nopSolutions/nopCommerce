using FluentMigrator;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Logging;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Vendors;
using Nop.Data.Extensions;

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

        //MFA #475
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
        //add column
        this.AddOrAlterColumnFor<ProductAttributeCombination>(t => t.MinStockQuantity)
        .AsInt32()
        .NotNullable()
        .SetExistingRowsTo(0);

        //#276 AJAX filters
        //remove column
        this.DeleteColumnsIfExists<Category>(["PriceRanges"]);
        this.DeleteColumnsIfExists<Manufacturer>(["PriceRanges"]);

        //add column
        this.AddOrAlterColumnFor<Category>(t => t.PriceRangeFiltering)
            .AsBoolean()
            .NotNullable()
            .SetExistingRowsTo(true);

        this.AddOrAlterColumnFor<Manufacturer>(t => t.PriceRangeFiltering)
            .AsBoolean()
            .NotNullable()
            .SetExistingRowsTo(true);

        this.AddOrAlterColumnFor<Vendor>(t => t.PriceRangeFiltering)
            .AsBoolean()
            .NotNullable()
            .SetExistingRowsTo(true);

        //add column
        this.AddOrAlterColumnFor<Category>(t => t.PriceFrom)
            .AsDecimal()
            .NotNullable()
            .SetExistingRowsTo(0);
        this.AddOrAlterColumnFor<Manufacturer>(t => t.PriceFrom)
            .AsDecimal()
            .NotNullable()
            .SetExistingRowsTo(0);
        this.AddOrAlterColumnFor<Vendor>(t => t.PriceFrom)
            .AsDecimal()
            .NotNullable()
            .SetExistingRowsTo(0);

        //add column
        this.AddOrAlterColumnFor<Category>(t => t.PriceTo)
            .AsDecimal()
            .NotNullable()
            .SetExistingRowsTo(10000);
        this.AddOrAlterColumnFor<Manufacturer>(t => t.PriceTo)
            .AsDecimal()
            .NotNullable()
            .SetExistingRowsTo(10000);
        this.AddOrAlterColumnFor<Vendor>(t => t.PriceTo)
            .AsDecimal()
            .NotNullable()
            .SetExistingRowsTo(10000);

        //add column
        this.AddOrAlterColumnFor<Category>(t => t.ManuallyPriceRange)
            .AsBoolean()
            .NotNullable()
            .SetExistingRowsTo(false);
        this.AddOrAlterColumnFor<Manufacturer>(t => t.ManuallyPriceRange)
            .AsBoolean()
            .NotNullable()
            .SetExistingRowsTo(false);
        this.AddOrAlterColumnFor<Vendor>(t => t.ManuallyPriceRange)
            .AsBoolean()
            .NotNullable()
            .SetExistingRowsTo(false);
    }

    public override void Down()
    {
        //add the downgrade logic if necessary 
    }
}