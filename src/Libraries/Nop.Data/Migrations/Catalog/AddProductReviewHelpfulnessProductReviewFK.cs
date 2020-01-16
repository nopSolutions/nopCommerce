using System.Data;
using FluentMigrator;
using Nop.Core.Domain.Catalog;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Catalog
{
    [Migration(637097639558603530)]
    public class AddProductReviewHelpfulnessProductReviewFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(nameof(ProductReviewHelpfulness),
                nameof(ProductReviewHelpfulness.ProductReviewId),
                nameof(ProductReview),
                nameof(ProductReview.Id),
                Rule.Cascade);
        }

        #endregion
    }
}