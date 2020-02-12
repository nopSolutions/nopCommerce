using System.Data;
using FluentMigrator;
using Nop.Core.Domain.Catalog;
using Nop.Data.Extensions;
using Nop.Data.Mapping;

namespace Nop.Data.Migrations.Catalog
{
    [NopMigration("2019/11/19 12:46:00:2513442")]
    public class AddProductReviewReviewTypeMappingReviewTypeFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(NopMappingDefaults.ProductReviewReviewTypeTable, 
                nameof(ProductReviewReviewTypeMapping.ReviewTypeId), 
                nameof(ReviewType), 
                nameof(ReviewType.Id), 
                Rule.Cascade);
        }
        
        #endregion
    }
}
