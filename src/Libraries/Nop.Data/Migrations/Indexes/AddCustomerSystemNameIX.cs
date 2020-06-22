using FluentMigrator;
using Nop.Core.Domain.Customers;

namespace Nop.Data.Migrations.Indexes
{
    [NopMigration("2020/03/13 09:36:08:9037684")]
    public class AddCustomerSystemNameIX : AutoReversingMigration
    {
        #region Methods          

        public override void Up()
        {
            Create.Index("IX_Customer_SystemName").OnTable(nameof(Customer))
                .OnColumn(nameof(Customer.SystemName)).Ascending()
                .WithOptions().NonClustered();
        }

        #endregion
    }
}