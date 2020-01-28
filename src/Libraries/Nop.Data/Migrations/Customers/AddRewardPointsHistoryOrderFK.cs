using System.Data;
using FluentMigrator;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Customers
{
    [NopMigration("2019/11/19 02:35:25:2342367")]
    public class AddRewardPointsHistoryOrderFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(nameof(RewardPointsHistory),
                nameof(RewardPointsHistory.OrderId),
                nameof(Order),
                nameof(Order.Id),
                Rule.SetNull);
        }

        #endregion
    }
}