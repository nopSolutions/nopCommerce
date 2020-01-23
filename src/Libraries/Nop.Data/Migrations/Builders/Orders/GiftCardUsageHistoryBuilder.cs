using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Orders;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Builders
{
    public partial class GiftCardUsageHistoryBuilder : BaseEntityBuilder<GiftCardUsageHistory>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
               .WithColumn(nameof(GiftCardUsageHistory.GiftCardId))
                   .AsInt32()
                   .ForeignKey<GiftCard>()
                .WithColumn(nameof(GiftCardUsageHistory.UsedWithOrderId))
                   .AsInt32()
                   .ForeignKey<Order>();
        }

        #endregion
    }
}