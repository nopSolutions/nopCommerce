using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Catalog;

namespace Nop.Data.Mapping.Catalog
{
    /// <summary>
    /// Represents a product attribute value mapping configuration
    /// </summary>
    public partial class ProductAttributeValueMap : NopEntityTypeConfiguration<ProductAttributeValue>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityTypeBuilder<ProductAttributeValue> builder)
        {
            builder.ToTable(nameof(ProductAttributeValue));
            builder.HasKey(value => value.Id);

            builder.Property(value => value.Name).HasMaxLength(400).IsRequired();
            builder.Property(value => value.ColorSquaresRgb).HasMaxLength(100);
            builder.Property(value => value.PriceAdjustment).HasColumnType("decimal(18, 4)");
            builder.Property(value => value.WeightAdjustment).HasColumnType("decimal(18, 4)");
            builder.Property(value => value.Cost).HasColumnType("decimal(18, 4)");

            builder.HasOne(value => value.ProductAttributeMapping)
                .WithMany(productAttributeMapping => productAttributeMapping.ProductAttributeValues)
                .HasForeignKey(value => value.ProductAttributeMappingId)
                .IsRequired();

            builder.Ignore(value => value.AttributeValueType);

            base.Configure(builder);
        }

        #endregion
    }
}