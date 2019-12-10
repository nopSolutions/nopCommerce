using FluentMigrator;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Orders;

namespace Nop.Data.Migrations.Orders
{
    [Migration(637097805896028944)]
    public class AddOrderShippingAddressFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            Create.ForeignKey().FromTable(nameof(Order))
                .ForeignColumn(nameof(Order.ShippingAddressId))
                .ToTable(nameof(Address))
                .PrimaryColumn(nameof(Address.Id));

            Create.Index().OnTable(nameof(Order)).OnColumn(nameof(Order.ShippingAddressId)).Ascending().WithOptions().NonClustered();
        }

        #endregion
    }
}