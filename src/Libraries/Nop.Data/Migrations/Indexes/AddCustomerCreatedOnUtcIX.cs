using FluentMigrator;
using Nop.Core.Domain.Customers;

namespace Nop.Data.Migrations.Indexes
{
    [NopMigration("2020/03/13 09:36:08:9037685")]
    public class AddCustomerCreatedOnUtcIX : AutoReversingMigration
    {
        #region Methods          

        public override void Up()
        {
            Create.Index("IX_Customer_CreatedOnUtc").OnTable(nameof(Customer))
                .OnColumn(nameof(Customer.CreatedOnUtc)).Descending()
                .WithOptions().NonClustered();
        }

        #endregion
    }
}