using FluentMigrator;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Logging;

namespace Nop.Data.Migrations.Logging
{
    [Migration(637097795893561926)]
    public class AddLogCustomerFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            Create.ForeignKey().FromTable(nameof(Log))
                .ForeignColumn(nameof(Log.CustomerId))
                .ToTable(nameof(Customer))
                .PrimaryColumn(nameof(Customer.Id));
        }

        #endregion
    }
}