using FluentMigrator;
using Nop.Core.Domain.Discounts;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Discounts
{
    [Migration(637118390520043561)]
    public class AddDiscountRequirementDiscountRequirementFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(nameof(DiscountRequirement),
                nameof(DiscountRequirement.DiscountId),
                nameof(DiscountRequirement),
                nameof(DiscountRequirement.Id));
        }

        #endregion
    }
}
