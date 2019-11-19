using FluentMigrator;
using Nop.Core.Domain.Catalog;

namespace Nop.Data.Migrations.Catalog
{
    [Migration(637097639998948304)]
    public class AddProductReviewProductFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            Create.ForeignKey().FromTable(nameof(ProductReview))
                .ForeignColumn(nameof(ProductReview.ProductId))
                .ToTable(nameof(Product))
                .PrimaryColumn(nameof(Product.Id));
        }

        #endregion
    }
}