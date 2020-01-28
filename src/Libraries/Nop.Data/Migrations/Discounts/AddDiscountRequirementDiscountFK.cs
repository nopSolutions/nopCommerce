using System.Data;
using FluentMigrator;
using Nop.Core.Domain.Discounts;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Discounts
{
    [NopMigration("2019/12/13 01:04:12:0043560")]
    public class AddDiscountRequirementDiscountFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(nameof(DiscountRequirement),
                nameof(DiscountRequirement.DiscountId),
                nameof(Discount),
                nameof(Discount.Id),
                Rule.Cascade);
        }

        #endregion
    }
}
