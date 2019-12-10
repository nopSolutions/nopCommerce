using System.Data;
using FluentMigrator;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;

namespace Nop.Data.Migrations.Customers
{
    [Migration(637097709252342367)]
    public class AddRewardPointsHistoryOrderFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            Create.ForeignKey().FromTable(nameof(RewardPointsHistory))
                .ForeignColumn(nameof(RewardPointsHistory.OrderId))
                .ToTable(nameof(Order))
                .PrimaryColumn(nameof(Order.Id))
                .OnDelete(Rule.SetNull);

            Create.Index().OnTable(nameof(RewardPointsHistory)).OnColumn(nameof(RewardPointsHistory.OrderId)).Ascending().WithOptions().NonClustered();
        }

        #endregion
    }
}