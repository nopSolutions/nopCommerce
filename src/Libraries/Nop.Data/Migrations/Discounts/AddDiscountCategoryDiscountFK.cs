using FluentMigrator;
using Nop.Core.Domain.Discounts;

namespace Nop.Data.Migrations.Discounts
{
    [Migration(637097771695936887)]
    public class AddDiscountCategoryDiscountFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            Create.ForeignKey().FromTable(NopMappingDefaults.DiscountAppliedToCategoriesTable)
                .ForeignColumn("Discount_Id")
                .ToTable(nameof(Discount))
                .PrimaryColumn(nameof(Discount.Id));

            Create.Index().OnTable(NopMappingDefaults.DiscountAppliedToCategoriesTable).OnColumn("Discount_Id").Ascending().WithOptions().NonClustered();
        }

        #endregion
    }
}