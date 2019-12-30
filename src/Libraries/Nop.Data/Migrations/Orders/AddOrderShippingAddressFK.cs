using FluentMigrator;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Orders;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Orders
{
    [Migration(637097805896028944)]
    public class AddOrderShippingAddressFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(nameof(Order),
                nameof(Order.ShippingAddressId),
                nameof(Address),
                nameof(Address.Id));
        }

        #endregion
    }
}