using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Catalog;
using Nop.Data.Extensions;

namespace Nop.Data.Mapping.Builders.Catalog
{
    /// <summary>
    /// Represents a product review helpfulness entity builder
    /// </summary>
    public partial class ProductReviewHelpfulnessBuilder : NopEntityBuilder<ProductReviewHelpfulness>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(ProductReviewHelpfulness.ProductReviewId)).AsInt32().ForeignKey<ProductReview>();
        }

        #endregion
    }
}