using LinqToDB.Mapping;
using Nop.Core.Domain.Catalog;
using Nop.Data.Data;

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
        public override void Configure(EntityMappingBuilder<ProductPicture> builder)
        {
            builder.HasTableName(NopMappingDefaults.ProductPictureTable);

            builder.Property(productpicture => productpicture.ProductId);
            builder.Property(productpicture => productpicture.PictureId);
            builder.Property(productpicture => productpicture.DisplayOrder);

            //TODO: 239 Try to add ForeignKey
            //builder.HasOne(productPicture => productPicture.Picture)
            //    .WithMany()
            //    .HasForeignKey(productPicture => productPicture.PictureId)
            //    .IsColumnRequired();

            //builder.HasOne(productPicture => productPicture.Product)
            //    .WithMany(product => product.ProductPictures)
            //    .HasForeignKey(productPicture => productPicture.ProductId)
            //    .IsColumnRequired();
        }

        #endregion
    }
}