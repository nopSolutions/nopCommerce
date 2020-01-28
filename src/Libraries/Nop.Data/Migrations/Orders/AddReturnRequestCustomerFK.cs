using System.Data;
using FluentMigrator;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Orders
{
    [NopMigration("2019/11/19 05:27:09:1248082")]
    public class AddReturnRequestCustomerFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(nameof(ReturnRequest),
                nameof(ReturnRequest.CustomerId),
                nameof(Customer),
                nameof(Customer.Id),
                Rule.Cascade);
        }

        #endregion
    }
}