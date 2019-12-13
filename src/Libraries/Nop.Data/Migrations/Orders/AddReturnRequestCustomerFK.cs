using System.Data;
using FluentMigrator;
using LinqToDB.Mapping;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Orders
{
    [Migration(637097812291248082)]
    public class AddReturnRequestCustomerFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(nameof(ReturnRequest)
                , nameof(ReturnRequest.CustomerId)
                , nameof(Customer)
                , nameof(Customer.Id)
                , Rule.Cascade);
        }

        #endregion
    }
}