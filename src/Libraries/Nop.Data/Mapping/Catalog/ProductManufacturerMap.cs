using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Catalog;

namespace Nop.Data.Mapping.Catalog
{
    /// <summary>
    /// Represents a product manufacturer mapping configuration
    /// </summary>
    public partial class ProductManufacturerMap : NopEntityTypeConfiguration<ProductManufacturer>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityTypeBuilder<ProductManufacturer> builder)
        {
            builder.ToTable(NopMappingDefaults.ProductManufacturerTable);
            builder.HasKey(productManufacturer => productManufacturer.Id);

            builder.HasOne(productManufacturer => productManufacturer.Manufacturer)
                .WithMany()
                .HasForeignKey(productManufacturer => productManufacturer.ManufacturerId)
                .IsRequired();

            builder.HasOne(productManufacturer => productManufacturer.Product)
                .WithMany(product => product.ProductManufacturers)
                .HasForeignKey(productManufacturer => productManufacturer.ProductId)
                .IsRequired();

            base.Configure(builder);
        }

        #endregion
    }
}