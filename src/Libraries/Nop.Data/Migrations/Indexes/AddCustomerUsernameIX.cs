using FluentMigrator;
using Nop.Core.Domain.Customers;

namespace Nop.Data.Migrations.Indexes
{
    [NopMigration("2020/03/13 09:36:08:9037682")]
    public class AddCustomerUsernameIX : AutoReversingMigration
    {
        #region Methods          

        public override void Up()
        {
            Create.Index("IX_Customer_Username").OnTable(nameof(Customer))
                .OnColumn(nameof(Customer.Username)).Ascending()
                .WithOptions().NonClustered();
        }

        #endregion
    }
}