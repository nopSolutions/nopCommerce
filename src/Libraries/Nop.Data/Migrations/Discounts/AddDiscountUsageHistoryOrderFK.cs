using System.Data;
using FluentMigrator;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Orders;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Discounts
{
    [NopMigration("2019/11/19 04:33:24:1180784")]
    public class AddDiscountUsageHistoryOrderFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(nameof(DiscountUsageHistory),
                nameof(DiscountUsageHistory.OrderId),
                nameof(Order),
                nameof(Order.Id),
                Rule.Cascade);
        }

        #endregion
    }
}