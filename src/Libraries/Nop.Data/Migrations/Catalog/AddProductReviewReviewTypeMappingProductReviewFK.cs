using System.Data;
using FluentMigrator;
using Nop.Core.Domain.Catalog;

namespace Nop.Data.Migrations.Catalog
{
    [Migration(637097643602513441)]
    public class AddProductReviewReviewTypeMappingProductReviewFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            Create.ForeignKey().FromTable(NopMappingDefaults.ProductReviewReviewTypeTable)
                .ForeignColumn(nameof(ProductReviewReviewTypeMapping.ProductReviewId))
                .ToTable(nameof(ProductReview))
                .PrimaryColumn(nameof(ProductReview.Id))
                .OnDelete(Rule.Cascade);

            Create.Index().OnTable(NopMappingDefaults.ProductReviewReviewTypeTable).OnColumn(nameof(ProductReviewReviewTypeMapping.ProductReviewId)).Ascending().WithOptions().NonClustered();
        }

        #endregion
    }
}