using System.Data;
using FluentMigrator;
using Nop.Core.Domain.Customers;
using Nop.Data.Extensions;
using Nop.Data.Mapping;

namespace Nop.Data.Migrations.Security
{
    [NopMigration("2019/11/19 05:38:30:7801301")]
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