using FluentMigrator;
using Nop.Core.Domain.Catalog;

namespace Nop.Data.Migrations.Discounts
{
    [Migration(637097778951975257)]
    public class AddDiscountProductProductFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            Create.ForeignKey().FromTable(NopMappingDefaults.DiscountAppliedToProductsTable)
                .ForeignColumn("Product_Id")
                .ToTable(nameof(Product))
                .PrimaryColumn(nameof(Product.Id));
        }

        #endregion
    }
}