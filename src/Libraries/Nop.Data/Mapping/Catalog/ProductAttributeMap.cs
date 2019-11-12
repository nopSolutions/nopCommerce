using LinqToDB.Mapping;
using Nop.Core.Domain.Catalog;

namespace Nop.Data.Mapping.Catalog
{
    /// <summary>
    /// Represents a product attribute mapping configuration
    /// </summary>
    public partial class ProductAttributeMap : NopEntityTypeConfiguration<ProductAttribute>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityMappingBuilder<ProductAttribute> builder)
        {
            builder.HasTableName(nameof(ProductAttribute));

            builder.HasColumn(attribute => attribute.Name).IsColumnRequired();
            builder.Property(productattribute => productattribute.Description);
        }

        #endregion
    }
}