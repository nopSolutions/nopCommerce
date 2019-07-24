using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Catalog;

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

            builder.HasOne(productPicture => productPicture.Picture)
                .WithMany()
                .HasForeignKey(productPicture => productPicture.PictureId)
                .IsRequired();

            builder.HasOne(productPicture => productPicture.Product)
                .WithMany(product => product.ProductPictures)
                .HasForeignKey(productPicture => productPicture.ProductId)
                .IsRequired();

            base.Configure(builder);
        }

        #endregion
    }
}