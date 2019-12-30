using FluentMigrator;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Customers
{
    [Migration(637097705651641382)]
    public class AddCustomerShippingAddressFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(nameof(Customer),
                "ShippingAddress_Id",
                nameof(Address),
                nameof(Address.Id));
        }

        #endregion
    }
}