using System.Data;
using FluentMigrator;
using Nop.Core.Domain.Security;
using Nop.Data.Extensions;
using Nop.Data.Mapping;

namespace Nop.Data.Migrations.Security
{
    [NopMigration("2019/11/19 05:38:30:7801302")]
    public class AddPermissionRecordCustomerRolePermissionRecordFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(NopMappingDefaults.PermissionRecordRoleTable,
                "PermissionRecord_Id",
                nameof(PermissionRecord),
                nameof(PermissionRecord.Id),
                Rule.Cascade);
        }

        #endregion
    }
}