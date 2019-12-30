using System.Data;
using FluentMigrator;
using Nop.Core.Domain.Customers;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Customers
{
    [Migration(637097709252342366)]
    public class AddRewardPointsHistoryCustomerFK : AutoReversingMigration
    {
        #region Methods
        
        public override void Up()
        {
            this.AddForeignKey(nameof(RewardPointsHistory),
                nameof(RewardPointsHistory.CustomerId),
                nameof(Customer),
                nameof(Customer.Id),
                Rule.Cascade);
        }

        #endregion
    }
}