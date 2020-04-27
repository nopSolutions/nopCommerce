using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Orders;
using Nop.Data.Extensions;

namespace Nop.Data.Mapping.Builders.Discounts
{
    /// <summary>
    /// Represents a discount usage history entity builder
    /// </summary>
    public partial class DiscountUsageHistoryBuilder : NopEntityBuilder<DiscountUsageHistory>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(DiscountUsageHistory.DiscountId)).AsInt32().ForeignKey<Discount>()
                .WithColumn(nameof(DiscountUsageHistory.OrderId)).AsInt32().ForeignKey<Order>();
        }

        #endregion
    }
}