using FluentMigrator;
using Nop.Core.Domain.Catalog;

namespace Nop.Data.Migrations.Catalog
{
    [Migration(637097643602513442)]
    public class AddProductReviewReviewTypeMappingReviewTypeFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            Create.ForeignKey().FromTable(NopMappingDefaults.ProductReviewReviewTypeTable)
                .ForeignColumn(nameof(ProductReviewReviewTypeMapping.ReviewTypeId))
                .ToTable(nameof(ReviewType))
                .PrimaryColumn(nameof(ReviewType.Id));
        }
        
        #endregion
    }
}
