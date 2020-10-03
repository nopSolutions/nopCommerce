using System.Linq;
using FluentMigrator;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Logging;
using Nop.Core.Domain.Security;

namespace Nop.Data.Migrations.UpgradeTo440
{
    [NopMigration("2020-06-10 00:00:00", "4.40.0", UpdateMigrationType.Data)]
    [SkipMigrationOnInstall]
    public class DataMigration : MigrationBase
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
        }

        public override void Down()
        {
            //add the downgrade logic if necessary 
        }
    }
}
