using FluentMigrator;
using Nop.Core.Domain.Orders;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Orders
{
    [Migration(637097802922130581)]
    public class AddGiftCardOrderItemFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(nameof(GiftCard)
                , nameof(GiftCard.PurchasedWithOrderItemId)
                , nameof(OrderItem)
                , nameof(OrderItem.Id));
        }

        #endregion
    }
}