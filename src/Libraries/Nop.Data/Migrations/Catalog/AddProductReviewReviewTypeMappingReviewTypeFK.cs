using System.Data;
using FluentMigrator;
using Nop.Core.Domain.Catalog;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Catalog
{
    [Migration(637097643602513442)]
    public class AddProductReviewReviewTypeMappingReviewTypeFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(NopMappingDefaults.ProductReviewReviewTypeTable
                , nameof(ProductReviewReviewTypeMapping.ReviewTypeId)
                , nameof(ReviewType)
                , nameof(ReviewType.Id)
                , Rule.Cascade);
        }
        
        #endregion
    }
}
