using System.Data;
using FluentMigrator;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Security;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Security
{
    [Migration(637097818436073081)]
    public class AddAclRecordCustomerRoleFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(nameof(AclRecord)
                , nameof(AclRecord.CustomerRoleId)
                , nameof(CustomerRole)
                , nameof(CustomerRole.Id)
                , Rule.Cascade);
        }

        #endregion
    }
}