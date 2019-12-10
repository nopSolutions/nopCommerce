using FluentMigrator;
using Nop.Core.Domain.Discounts;

namespace Nop.Data.Migrations.Discounts
{
    [Migration(637097780041180783)]
    public class AddDiscountUsageHistoryDiscountFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            Create.ForeignKey().FromTable(nameof(DiscountUsageHistory))
                .ForeignColumn(nameof(DiscountUsageHistory.DiscountId))
                .ToTable(nameof(Discount))
                .PrimaryColumn(nameof(Discount.Id));

            Create.Index().OnTable(nameof(DiscountUsageHistory)).OnColumn(nameof(DiscountUsageHistory.DiscountId)).Ascending().WithOptions().NonClustered();
        }

        #endregion
    }
}