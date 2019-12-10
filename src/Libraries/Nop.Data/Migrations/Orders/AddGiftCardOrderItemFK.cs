using FluentMigrator;
using Nop.Core.Domain.Orders;

namespace Nop.Data.Migrations.Orders
{
    [Migration(637097802922130581)]
    public class AddGiftCardOrderItemFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            Create.ForeignKey().FromTable(nameof(GiftCard))
                .ForeignColumn(nameof(GiftCard.PurchasedWithOrderItemId))
                .ToTable(nameof(OrderItem))
                .PrimaryColumn(nameof(OrderItem.Id));

            Create.Index().OnTable(nameof(GiftCard)).OnColumn(nameof(GiftCard.PurchasedWithOrderItemId)).Ascending().WithOptions().NonClustered();
        }

        #endregion
    }
}