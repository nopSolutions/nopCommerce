using FluentMigrator;
using Nop.Core.Domain.Orders;

namespace Nop.Data.Migrations.Orders
{
    [Migration(637097803156452476)]
    public class AddGiftCardUsageHistoryOrderFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            Create.ForeignKey().FromTable(nameof(GiftCardUsageHistory))
                .ForeignColumn(nameof(GiftCardUsageHistory.UsedWithOrderId))
                .ToTable(nameof(Order))
                .PrimaryColumn(nameof(Order.Id));

            Create.Index().OnTable(nameof(GiftCardUsageHistory)).OnColumn(nameof(GiftCardUsageHistory.UsedWithOrderId)).Ascending().WithOptions().NonClustered();
        }

        #endregion
    }
}