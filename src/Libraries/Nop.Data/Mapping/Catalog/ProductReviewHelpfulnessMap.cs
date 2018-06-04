using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Catalog;

namespace Nop.Data.Mapping.Catalog
{
    /// <summary>
    /// Represents a product review helpfulness mapping configuration
    /// </summary>
    public partial class ProductReviewHelpfulnessMap : NopEntityTypeConfiguration<ProductReviewHelpfulness>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityTypeBuilder<ProductReviewHelpfulness> builder)
        {
            builder.ToTable(nameof(ProductReviewHelpfulness));
            builder.HasKey(productReviewHelpfulness => productReviewHelpfulness.Id);

            builder.HasOne(productReviewHelpfulness => productReviewHelpfulness.ProductReview)
                .WithMany(productReview => productReview.ProductReviewHelpfulnessEntries)
                .HasForeignKey(productReviewHelpfulness => productReviewHelpfulness.ProductReviewId)
                .IsRequired();

            base.Configure(builder);
        }

        #endregion
    }
}