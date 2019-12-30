using System.Data;
using FluentMigrator;
using Nop.Core.Domain.Customers;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Security
{
    [Migration(637097819107801301)]
    public class AddPermissionRecordCustomerRoleCustomerRoleFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(NopMappingDefaults.PermissionRecordRoleTable,
                "CustomerRole_Id",
                nameof(CustomerRole),
                nameof(CustomerRole.Id),
                Rule.Cascade);
        }

        #endregion
    }
}