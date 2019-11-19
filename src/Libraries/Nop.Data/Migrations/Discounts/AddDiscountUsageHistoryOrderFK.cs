using FluentMigrator;
using LinqToDB.Mapping;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Orders;

namespace Nop.Data.Migrations.Discounts
{
    [Migration(637097780041180784)]
    public class AddDiscountUsageHistoryOrderFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            Create.ForeignKey().FromTable(nameof(DiscountUsageHistory))
                .ForeignColumn(nameof(DiscountUsageHistory.OrderId))
                .ToTable(nameof(Order))
                .PrimaryColumn(nameof(Order.Id));
        }

        #endregion
    }
}