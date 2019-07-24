using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Catalog;

namespace Nop.Data.Mapping.Catalog
{
    /// <summary>
    /// Represents a product specification attribute mapping configuration
    /// </summary>
    public partial class ProductSpecificationAttributeMap : NopEntityTypeConfiguration<ProductSpecificationAttribute>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityTypeBuilder<ProductSpecificationAttribute> builder)
        {
            builder.ToTable(NopMappingDefaults.ProductSpecificationAttributeTable);
            builder.HasKey(productSpecificationAttribute => productSpecificationAttribute.Id);

            builder.Property(productSpecificationAttribute => productSpecificationAttribute.CustomValue).HasMaxLength(4000);

            builder.HasOne(productSpecificationAttribute => productSpecificationAttribute.SpecificationAttributeOption)
                .WithMany()
                .HasForeignKey(productSpecificationAttribute => productSpecificationAttribute.SpecificationAttributeOptionId)
                .IsRequired();

            builder.HasOne(productSpecificationAttribute => productSpecificationAttribute.Product)
                .WithMany(product => product.ProductSpecificationAttributes)
                .HasForeignKey(productSpecificationAttribute => productSpecificationAttribute.ProductId)
                .IsRequired();

            builder.Ignore(productSpecificationAttribute => productSpecificationAttribute.AttributeType);

            base.Configure(builder);
        }

        #endregion
    }
}