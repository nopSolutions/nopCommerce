using FluentMigrator;
using Nop.Core.Domain.Customers;

namespace Nop.Data.Migrations.Indexes
{
    [NopMigration("2020/03/13 09:36:08:9037683")]
    public class AddCustomerCustomerGuidIX : AutoReversingMigration
    {
        #region Methods          

        public override void Up()
        {
            Create.Index("IX_Customer_CustomerGuid").OnTable(nameof(Customer))
                .OnColumn(nameof(Customer.CustomerGuid)).Ascending()
                .WithOptions().NonClustered();
        }

        #endregion
    }
}