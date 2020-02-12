using System.Data;
using FluentMigrator;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Orders
{
    [NopMigration("2019/11/19 05:16:29:6028942")]
    public class AddOrderCustomerFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(nameof(Order),
                nameof(Order.CustomerId),
                nameof(Customer),
                nameof(Customer.Id),
                Rule.None);
        }

        #endregion
    }
}