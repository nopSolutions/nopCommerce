using System.Data;
using FluentMigrator;
using Nop.Core.Domain.Catalog;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Catalog
{
    [Migration(637097643602513441)]
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