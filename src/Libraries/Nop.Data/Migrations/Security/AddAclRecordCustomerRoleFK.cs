using FluentMigrator;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Security;

namespace Nop.Data.Migrations.Security
{
    [Migration(637097818436073081)]
    public class AddAclRecordCustomerRoleFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            Create.ForeignKey().FromTable(nameof(AclRecord))
                .ForeignColumn(nameof(AclRecord.CustomerRoleId))
                .ToTable(nameof(CustomerRole))
                .PrimaryColumn(nameof(CustomerRole.Id));
        }

        #endregion
    }
}