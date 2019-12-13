using System.Data;
using FluentMigrator;
using Nop.Core.Domain.Orders;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Orders
{
    [Migration(637097803156452476)]
    public class AddGiftCardUsageHistoryOrderFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(nameof(GiftCardUsageHistory)
                , nameof(GiftCardUsageHistory.UsedWithOrderId)
                , nameof(Order)
                , nameof(Order.Id)
                , Rule.Cascade);
        }

        #endregion
    }
}