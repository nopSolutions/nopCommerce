using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Media;

namespace Nop.Data.Mapping.Catalog
{
    /// <summary>
    /// Represents a product picture mapping configuration
    /// </summary>
    public partial class ProductPictureMap : NopEntityTypeConfiguration<ProductPicture>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityTypeBuilder<ProductPicture> builder)
        {
            builder.ToTable(NopMappingDefaults.ProductPictureTable);
            builder.HasKey(productPicture => productPicture.Id);

            builder.HasOne<Picture>().WithMany().HasForeignKey(productPicture => productPicture.PictureId).IsRequired();

            builder.HasOne<Product>().WithMany().HasForeignKey(productPicture => productPicture.ProductId).IsRequired();

            base.Configure(builder);
        }

        #endregion
    }
}