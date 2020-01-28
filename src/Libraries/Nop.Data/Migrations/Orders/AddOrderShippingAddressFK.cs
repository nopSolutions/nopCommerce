using FluentMigrator;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Orders;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Orders
{
    [NopMigration("2019/11/19 05:16:29:6028944")]
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