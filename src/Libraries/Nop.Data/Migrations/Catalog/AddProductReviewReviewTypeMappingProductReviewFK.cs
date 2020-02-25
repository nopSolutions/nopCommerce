using System.Data;
using FluentMigrator;
using Nop.Core.Domain.Catalog;
using Nop.Data.Extensions;
using Nop.Data.Mapping;

namespace Nop.Data.Migrations.Catalog
{
    [NopMigration("2019/11/19 12:46:00:2513441")]
    public class AddProductReviewReviewTypeMappingProductReviewFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(NopMappingDefaults.ProductReviewReviewTypeTable, 
                nameof(ProductReviewReviewTypeMapping.ProductReviewId), 
                nameof(ProductReview), 
                nameof(ProductReview.Id), 
                Rule.Cascade);
        }

        #endregion
    }
}