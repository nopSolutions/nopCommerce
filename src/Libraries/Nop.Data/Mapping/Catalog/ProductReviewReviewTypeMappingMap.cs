using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;

namespace Nop.Data.Mapping.Catalog
{
    /// <summary>
    /// Mapping class
    /// </summary>
    public partial class ProductReviewReviewTypeMappingMap : NopEntityTypeConfiguration<ProductReviewReviewTypeMapping>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityTypeBuilder<ProductReviewReviewTypeMapping> builder)
        {
            builder.ToTable(NopMappingDefaults.ProductReviewReviewTypeTable);
            builder.HasKey(prrt => prrt.Id);

            builder.HasOne<ProductReview>().WithMany().HasForeignKey(prrt => prrt.ProductReviewId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne<ReviewType>().WithMany().HasForeignKey(prrt => prrt.ReviewTypeId)
                .OnDelete(DeleteBehavior.Cascade);

            base.Configure(builder);
        }

        #endregion
    }
}
