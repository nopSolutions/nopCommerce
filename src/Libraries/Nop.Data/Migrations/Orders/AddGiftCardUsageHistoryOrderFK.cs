using System.Data;
using FluentMigrator;
using Nop.Core.Domain.Orders;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Orders
{
    [NopMigration("2019/11/19 05:11:55:6452476")]
    public class AddGiftCardUsageHistoryOrderFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(nameof(GiftCardUsageHistory),
                nameof(GiftCardUsageHistory.UsedWithOrderId),
                nameof(Order),
                nameof(Order.Id),
                Rule.Cascade);
        }

        #endregion
    }
}