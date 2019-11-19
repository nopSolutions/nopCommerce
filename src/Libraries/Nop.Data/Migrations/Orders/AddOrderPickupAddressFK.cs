using FluentMigrator;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Orders;

namespace Nop.Data.Migrations.Orders
{
    [Migration(637097805896028945)]
    public class AddOrderPickupAddressFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            Create.ForeignKey().FromTable(nameof(Order))
                .ForeignColumn(nameof(Order.PickupAddressId))
                .ToTable(nameof(Address))
                .PrimaryColumn(nameof(Address.Id));
        }

        #endregion
    }
}