using System.Data;
using FluentMigrator;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Customers
{
    [Migration(637097709252342367)]
    public class AddRewardPointsHistoryOrderFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(nameof(RewardPointsHistory)
                , nameof(RewardPointsHistory.OrderId)
                , nameof(Order)
                , nameof(Order.Id)
                , Rule.SetNull);
        }

        #endregion
    }
}