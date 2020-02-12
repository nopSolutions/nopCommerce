using FluentMigrator;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Customers
{
    [NopMigration("2019/11/19 02:29:25:1641381")]
    public class AddCustomerBillingAddressFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(nameof(Customer),
                "BillingAddress_Id",
                nameof(Address),
                nameof(Address.Id));
        }

        #endregion
    }
}