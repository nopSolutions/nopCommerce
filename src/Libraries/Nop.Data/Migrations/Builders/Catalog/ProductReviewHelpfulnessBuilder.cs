using FluentMigrator.Builders.Create.Table;
using LinqToDB.Mapping;
using Nop.Core.Domain.Catalog;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Builders
{
    public partial class ProductReviewHelpfulnessBuilder : BaseEntityBuilder<ProductReviewHelpfulness>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(ProductReviewHelpfulness.ProductReviewId))
                .AsInt32()
                .ForeignKey<ProductReview>();
        }

        #endregion
    }
}