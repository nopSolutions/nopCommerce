using System.Data;
using FluentMigrator;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Security;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Security
{
    [NopMigration("2019/11/19 05:37:23:6073081")]
    public class AddAclRecordCustomerRoleFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(nameof(AclRecord),
                nameof(AclRecord.CustomerRoleId),
                nameof(CustomerRole),
                nameof(CustomerRole.Id),
                Rule.Cascade);
        }

        #endregion
    }
}