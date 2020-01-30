using LinqToDB.Mapping;
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
        public override void Configure(EntityMappingBuilder<ProductSpecificationAttribute> builder)
        {
            builder.HasTableName(NopMappingDefaults.ProductSpecificationAttributeTable);

            builder.Property(productSpecificationAttribute => productSpecificationAttribute.CustomValue).HasLength(4000);
            builder.Property(productspecificationattribute => productspecificationattribute.ProductId);
            builder.Property(productspecificationattribute => productspecificationattribute.AttributeTypeId);
            builder.Property(productspecificationattribute => productspecificationattribute.SpecificationAttributeOptionId);
            builder.Property(productspecificationattribute => productspecificationattribute.AllowFiltering);
            builder.Property(productspecificationattribute => productspecificationattribute.ShowOnProductPage);
            builder.Property(productspecificationattribute => productspecificationattribute.DisplayOrder);
          
            builder.Ignore(productSpecificationAttribute => productSpecificationAttribute.AttributeType);
        }

        #endregion
    }
}