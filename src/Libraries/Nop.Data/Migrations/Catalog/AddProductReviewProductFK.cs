using System.Data;
using FluentMigrator;
using Nop.Core.Domain.Catalog;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Catalog
{
    [Migration(637097639998948304)]
    public class AddProductReviewProductFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(nameof(ProductReview)
                , nameof(ProductReview.ProductId)
                , nameof(Product)
                , nameof(Product.Id)
                , Rule.Cascade);
        }

        #endregion
    }
}