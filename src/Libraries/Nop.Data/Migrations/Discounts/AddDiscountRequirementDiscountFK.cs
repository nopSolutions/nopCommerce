using System.Data;
using FluentMigrator;
using Nop.Core.Domain.Discounts;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Discounts
{
    [Migration(637118390520043560)]
    public class AddDiscountRequirementDiscountFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(nameof(DiscountRequirement)
                , nameof(DiscountRequirement.DiscountId)
                , nameof(Discount)
                , nameof(Discount.Id)
                , Rule.Cascade);
        }

        #endregion
    }
}
