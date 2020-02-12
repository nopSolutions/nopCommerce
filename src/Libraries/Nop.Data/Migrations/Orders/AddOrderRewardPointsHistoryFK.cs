using FluentMigrator;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Orders
{
    [NopMigration("2019/12/16 04:36:01:7140897")]
    public class AddOrderRewardPointsHistoryFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(nameof(Order),
                nameof(Order.RewardPointsHistoryEntryId),
                nameof(RewardPointsHistory),
                nameof(RewardPointsHistory.Id));
        }

        #endregion
    }
}