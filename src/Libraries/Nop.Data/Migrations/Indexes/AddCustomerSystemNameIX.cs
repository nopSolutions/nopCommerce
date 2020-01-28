using FluentMigrator;
using Nop.Core.Domain.Customers;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Indexes
{
    [NopMigration("2019/12/19 09:36:08:9037684")]
    public class AddCustomerSystemNameIX : AutoReversingMigration
    {
        #region Methods          

        public override void Up()
        {
            this.AddIndex("IX_Customer_SystemName", nameof(Customer), i => i.Ascending(), nameof(Customer.SystemName));
        }

        #endregion
    }
}