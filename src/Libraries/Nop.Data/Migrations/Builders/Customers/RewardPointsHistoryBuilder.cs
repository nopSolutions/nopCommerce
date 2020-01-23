using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Builders
{
    public partial class RewardPointsHistoryBuilder : BaseEntityBuilder<RewardPointsHistory>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(RewardPointsHistory.CustomerId))
                    .AsInt32()
                    .ForeignKey<Customer>()
                .WithColumn(nameof(RewardPointsHistory.OrderId))
                    .AsInt32()
                    .Nullable()
                    .ForeignKey<Order>();
        }

        #endregion
    }
}