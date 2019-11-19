using FluentMigrator;
using Nop.Core.Domain.Security;

namespace Nop.Data.Migrations.Security
{
    [Migration(637097819107801302)]
    public class AddPermissionRecordCustomerRolePermissionRecordFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            Create.ForeignKey().FromTable(NopMappingDefaults.PermissionRecordRoleTable)
                .ForeignColumn(nameof(PermissionRecordCustomerRoleMapping.PermissionRecordId))
                .ToTable(nameof(PermissionRecord))
                .PrimaryColumn(nameof(PermissionRecord.Id));
        }

        #endregion
    }
}