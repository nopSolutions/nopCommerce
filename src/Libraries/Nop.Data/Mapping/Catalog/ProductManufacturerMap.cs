using LinqToDB.Mapping;
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
        public override void Configure(EntityMappingBuilder<ProductManufacturer> builder)
        {
            builder.HasTableName(NopMappingDefaults.ProductManufacturerTable);

            builder.Property(productmanufacturer => productmanufacturer.ProductId);
            builder.Property(productmanufacturer => productmanufacturer.ManufacturerId);
            builder.Property(productmanufacturer => productmanufacturer.IsFeaturedProduct);
            builder.Property(productmanufacturer => productmanufacturer.DisplayOrder);
        }

        #endregion
    }
}