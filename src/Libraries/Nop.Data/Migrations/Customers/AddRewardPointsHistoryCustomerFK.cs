using FluentMigrator;
using Nop.Core.Domain.Customers;

namespace Nop.Data.Migrations.Customers
{
    [Migration(637097709252342366)]
    public class AddRewardPointsHistoryCustomerFK : AutoReversingMigration
    {
        #region Methods
        
        public override void Up()
        {
            Create.ForeignKey().FromTable(nameof(RewardPointsHistory))
                .ForeignColumn(nameof(RewardPointsHistory.CustomerId))
                .ToTable(nameof(Customer))
                .PrimaryColumn(nameof(Customer.Id));

            Create.Index().OnTable(nameof(RewardPointsHistory)).OnColumn(nameof(RewardPointsHistory.CustomerId)).Ascending().WithOptions().NonClustered();
        }

        #endregion
    }
}