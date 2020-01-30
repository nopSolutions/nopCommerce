using LinqToDB.Mapping;
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
        public override void Configure(EntityMappingBuilder<ProductReviewReviewTypeMapping> builder)
        {
            builder.HasTableName(NopMappingDefaults.ProductReviewReviewTypeTable);

            builder.Property(prrt => prrt.ProductReviewId);
            builder.Property(prrt => prrt.ReviewTypeId);
            builder.Property(prrt => prrt.Rating);        
        }

        #endregion
    }
}
