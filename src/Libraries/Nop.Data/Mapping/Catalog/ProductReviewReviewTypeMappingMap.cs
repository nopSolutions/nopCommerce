using LinqToDB.Mapping;
using Nop.Core.Domain.Catalog;
using Nop.Data.Data;

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
        public override void Configure(EntityMappingBuilder<ProductReviewReviewTypeMapping> builder)
        {
            builder.HasTableName(NopMappingDefaults.ProductReviewReviewTypeTable);

            builder.Property(prrt => prrt.ProductReviewId);
            builder.Property(prrt => prrt.ReviewTypeId);
            builder.Property(prrt => prrt.Rating);

            //TODO: 239 Try to add ForeignKey
            //builder.HasOne(prrt => prrt.ProductReview)
            //    .WithMany(r => r.ProductReviewReviewTypeMappingEntries)
            //    .HasForeignKey(prrt => prrt.ProductReviewId)
            //    .OnDelete(DeleteBehavior.Cascade);

            //builder.HasOne(pam => pam.ReviewType)
            //    .WithMany()
            //    .HasForeignKey(prrt => prrt.ReviewTypeId)
            //    .OnDelete(DeleteBehavior.Cascade);            
        }

        #endregion
    }
}
