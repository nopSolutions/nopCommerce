using FluentMigrator;
using Nop.Core.Domain.Discounts;

namespace Nop.Data.Migrations.Discounts
{
    [Migration(637097778951975256)]
    public class AddDiscountProductDiscountFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            Create.ForeignKey().FromTable(NopMappingDefaults.DiscountAppliedToProductsTable)
                .ForeignColumn("Discount_Id")
                .ToTable(nameof(Discount))
                .PrimaryColumn(nameof(Discount.Id));

            Create.Index().OnTable(NopMappingDefaults.DiscountAppliedToProductsTable).OnColumn("Discount_Id").Ascending().WithOptions().NonClustered();
        }

        #endregion
    }
}