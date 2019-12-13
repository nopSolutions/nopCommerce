using System.Data;
using FluentMigrator;
using Nop.Core.Domain.Discounts;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Discounts
{
    [Migration(637097771695936887)]
    public class AddDiscountCategoryDiscountFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(NopMappingDefaults.DiscountAppliedToCategoriesTable
                , "Discount_Id"
                , nameof(Discount)
                , nameof(Discount.Id)
                , Rule.Cascade);
        }

        #endregion
    }
}