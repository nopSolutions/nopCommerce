using FluentMigrator;
using Nop.Core.Domain.Customers;

namespace Nop.Data.Migrations.Indexes
{
    [NopMigration("2020/03/13 09:36:08:9037681")]
    public class AddCustomerEmailIX : AutoReversingMigration
    {
        #region Methods   

        public override void Up()
        {
            Create.Index("IX_Customer_Email").OnTable(nameof(Customer))
                .OnColumn(nameof(Customer.Email)).Ascending()
                .WithOptions().NonClustered();
        }

        #endregion
    }
}