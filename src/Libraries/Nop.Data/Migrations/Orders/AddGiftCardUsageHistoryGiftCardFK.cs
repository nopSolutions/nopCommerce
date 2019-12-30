using System.Data;
using FluentMigrator;
using Nop.Core.Domain.Orders;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Orders
{
    [Migration(637097803156452475)]
    public class AddGiftCardUsageHistoryGiftCardFK : AutoReversingMigration
    {
        #region Methods
        
        public override void Up()
        {
            this.AddForeignKey(nameof(GiftCardUsageHistory),
                nameof(GiftCardUsageHistory.GiftCardId),
                nameof(GiftCard),
                nameof(GiftCard.Id),
                Rule.Cascade);
        }

        #endregion
    }
}