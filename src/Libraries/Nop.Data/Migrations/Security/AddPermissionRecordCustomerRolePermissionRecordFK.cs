using System.Data;
using FluentMigrator;
using Nop.Core.Domain.Security;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Security
{
    [Migration(637097819107801302)]
    public class AddPermissionRecordCustomerRolePermissionRecordFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(NopMappingDefaults.PermissionRecordRoleTable
                , "PermissionRecord_Id"
                , nameof(PermissionRecord)
                , nameof(PermissionRecord.Id)
                , Rule.Cascade);
        }

        #endregion
    }
}