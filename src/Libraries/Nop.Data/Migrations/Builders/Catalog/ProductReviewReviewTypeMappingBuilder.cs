using FluentMigrator.Builders.Create.Table;
using LinqToDB.Mapping;
using Nop.Core.Domain.Catalog;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Builders
{
    public partial class ProductReviewReviewTypeMappingBuilder : BaseEntityBuilder<ProductReviewReviewTypeMapping>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(ProductReviewReviewTypeMapping.ProductReviewId))
                    .AsInt32()
                    .ForeignKey<ProductReview>()
                .WithColumn(nameof(ProductReviewReviewTypeMapping.ReviewTypeId))
                    .AsInt32()
                    .ForeignKey<ReviewType>();
        }

        #endregion
    }
}
