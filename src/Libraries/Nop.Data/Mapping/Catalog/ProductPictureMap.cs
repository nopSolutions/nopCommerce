using Nop.Core.Domain.Catalog;

namespace Nop.Data.Mapping.Catalog
{
    public partial class ProductPictureMap : NopEntityTypeConfiguration<ProductPicture>
    {
        public ProductPictureMap()
        {
            this.ToTable("Product_Picture_Mapping");
            this.HasKey(pp => pp.Id);
            
            this.HasRequired(pp => pp.Picture)
                .WithMany()
                .HasForeignKey(pp => pp.PictureId);


            this.HasRequired(pp => pp.Product)
                .WithMany(p => p.ProductPictures)
                .HasForeignKey(pp => pp.ProductId);
        }
    }
}