using FluentMigrator;
using Nop.Core.Domain.Catalog;

namespace Nop.Data.Migrations.Catalog
{
    [Migration(637097639558603530)]
    public  class AddProductReviewHelpfulnessProductReviewFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            Create.ForeignKey().FromTable(nameof(ProductReviewHelpfulness))
                .ForeignColumn(nameof(ProductReviewHelpfulness.ProductReviewId))
                .ToTable(nameof(ProductReview))
                .PrimaryColumn(nameof(ProductReview.Id));

            Create.Index().OnTable(nameof(ProductReviewHelpfulness)).OnColumn(nameof(ProductReviewHelpfulness.ProductReviewId)).Ascending().WithOptions().NonClustered();
        }

        #endregion
    }
}