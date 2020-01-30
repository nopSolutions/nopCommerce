using LinqToDB.Mapping;
using Nop.Core.Domain.Catalog;

namespace Nop.Data.Mapping.Catalog
{
    /// <summary>
    /// Represents a product category mapping configuration
    /// </summary>
    public partial class ProductCategoryMap : NopEntityTypeConfiguration<ProductCategory>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityMappingBuilder<ProductCategory> builder)
        {
            builder.HasTableName(NopMappingDefaults.ProductCategoryTable);

            builder.Property(productcategory => productcategory.ProductId);
            builder.Property(productcategory => productcategory.CategoryId);
            builder.Property(productcategory => productcategory.IsFeaturedProduct);
            builder.Property(productcategory => productcategory.DisplayOrder);
        }

        #endregion
    }
}