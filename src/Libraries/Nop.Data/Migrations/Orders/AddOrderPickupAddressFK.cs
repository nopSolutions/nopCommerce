using FluentMigrator;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Orders;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Orders
{
    [Migration(637097805896028945)]
    public class AddOrderPickupAddressFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(nameof(Order)
                , nameof(Order.PickupAddressId)
                , nameof(Address)
                , nameof(Address.Id));
        }

        #endregion
    }
}