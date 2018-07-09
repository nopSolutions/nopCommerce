using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Catalog;

namespace Nop.Data.Mapping.Catalog
{
    /// <summary>
    /// Represents a product-product tag mapping configuration
    /// </summary>
    public partial class ProductProductTagMap : NopEntityTypeConfiguration<ProductProductTagMapping>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityTypeBuilder<ProductProductTagMapping> builder)
        {
            builder.ToTable(NopMappingDefaults.ProductProductTagTable);
            builder.HasKey(mapping => new { mapping.ProductId, mapping.ProductTagId});

            builder.Property(mapping => mapping.ProductId).HasColumnName("Product_Id");
            builder.Property(mapping => mapping.ProductTagId).HasColumnName("ProductTag_Id");

            builder.HasOne(mapping => mapping.Product)
                .WithMany(product => product.ProductProductTagMappings)
                .HasForeignKey(mapping => mapping.ProductId)
                .IsRequired();

            builder.HasOne(mapping => mapping.ProductTag)
                .WithMany(productTag => productTag.ProductProductTagMappings)
                .HasForeignKey(mapping => mapping.ProductTagId)
                .IsRequired();

            builder.Ignore(mapping => mapping.Id);

            base.Configure(builder);
        }

        #endregion
    }
}