using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Orders;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Builders
{
    public partial class DiscountUsageHistoryBuilder : BaseEntityBuilder<DiscountUsageHistory>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(DiscountUsageHistory.DiscountId))
                   .AsInt32()
                   .ForeignKey<Discount>()
                .WithColumn(nameof(DiscountUsageHistory.OrderId))
                   .AsInt32()
                   .ForeignKey<Order>();
        }

        #endregion
    }
}