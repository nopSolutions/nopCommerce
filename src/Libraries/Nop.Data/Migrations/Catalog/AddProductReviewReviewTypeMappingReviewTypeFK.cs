using System.Data;
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
                .PrimaryColumn(nameof(ReviewType.Id))
                .OnDelete(Rule.Cascade);

            Create.Index().OnTable(NopMappingDefaults.ProductReviewReviewTypeTable).OnColumn(nameof(ProductReviewReviewTypeMapping.ReviewTypeId)).Ascending().WithOptions().NonClustered();
        }
        
        #endregion
    }
}
