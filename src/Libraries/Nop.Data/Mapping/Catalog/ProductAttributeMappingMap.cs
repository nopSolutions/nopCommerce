using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Catalog;

namespace Nop.Data.Mapping.Catalog
{
    /// <summary>
    /// Represents a product product attribute mapping configuration
    /// </summary>
    public partial class ProductAttributeMappingMap : NopEntityTypeConfiguration<ProductAttributeMapping>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityTypeBuilder<ProductAttributeMapping> builder)
        {
            builder.ToTable(NopMappingDefaults.ProductProductAttributeTable);
            builder.HasKey(productAttributeMapping => productAttributeMapping.Id);

            builder.HasOne(productAttributeMapping => productAttributeMapping.Product)
                .WithMany(product => product.ProductAttributeMappings)
                .HasForeignKey(productAttributeMapping => productAttributeMapping.ProductId)
                .IsRequired();

            builder.HasOne(productAttributeMapping => productAttributeMapping.ProductAttribute)
                .WithMany()
                .HasForeignKey(productAttributeMapping => productAttributeMapping.ProductAttributeId)
                .IsRequired();

            builder.Ignore(pam => pam.AttributeControlType);

            base.Configure(builder);
        }

        #endregion
    }
}