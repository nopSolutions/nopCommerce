using FluentMigrator;
using Nop.Core.Domain.Orders;

namespace Nop.Data.Migrations.Orders
{
    [Migration(637097803156452475)]
    public class AddGiftCardUsageHistoryGiftCardFK : AutoReversingMigration
    {
        #region Methods
        
        public override void Up()
        {
            Create.ForeignKey().FromTable(nameof(GiftCardUsageHistory))
                .ForeignColumn(nameof(GiftCardUsageHistory.GiftCardId))
                .ToTable(nameof(GiftCard))
                .PrimaryColumn(nameof(GiftCard.Id));
        }

        #endregion
    }
}