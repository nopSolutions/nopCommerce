using FluentMigrator;
using LinqToDB.Mapping;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;

namespace Nop.Data.Migrations.Orders
{
    [Migration(637097812291248082)]
    public class AddReturnRequestCustomerFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            Create.ForeignKey().FromTable(nameof(ReturnRequest))
                .ForeignColumn(nameof(ReturnRequest.CustomerId))
                .ToTable(nameof(Customer))
                .PrimaryColumn(nameof(Customer.Id));
        }

        #endregion
    }
}