using System.Data;
using FluentMigrator;
using Nop.Core.Domain.Discounts;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Discounts
{
    [Migration(637097780041180783)]
    public class AddDiscountUsageHistoryDiscountFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(nameof(DiscountUsageHistory),
                nameof(DiscountUsageHistory.DiscountId),
                nameof(Discount),
                nameof(Discount.Id),
                Rule.Cascade);
        }

        #endregion
    }
}