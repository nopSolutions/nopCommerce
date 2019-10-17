using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
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
        public override void Configure(EntityTypeBuilder<ProductCategory> builder)
        {
            builder.ToTable(NopMappingDefaults.ProductCategoryTable);
            builder.HasKey(productCategory => productCategory.Id);

            builder.HasOne<Category>().WithMany().HasForeignKey(productCategory => productCategory.CategoryId).IsRequired();

            builder.HasOne<Product>().WithMany().HasForeignKey(productCategory => productCategory.ProductId).IsRequired();

            base.Configure(builder);
        }

        #endregion
    }
}