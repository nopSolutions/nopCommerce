using FluentMigrator;
using Nop.Core.Domain.Customers;

namespace Nop.Data.Migrations.Indexes
{
    [NopMigration("2020/03/13 09:36:09:9037685")]
    public class AddCustomerCustomerOpenIdIX : AutoReversingMigration
    {
        #region Methods          

        public override void Up()
        {
            Create.Index("IX_Customer_CustomerOpenId").OnTable(nameof(Customer))
                .OnColumn(nameof(Customer.OpenId)).Ascending()
                .WithOptions().NonClustered();
        }

        #endregion
    }
}