using FluentMigrator;
using Nop.Core.Domain.Discounts;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Discounts
{
    [NopMigration("2019/12/13 01:04:12:0043561")]
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
