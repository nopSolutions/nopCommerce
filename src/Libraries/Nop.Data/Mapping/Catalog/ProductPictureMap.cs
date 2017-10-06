using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Catalog;

namespace Nop.Data.Mapping.Catalog
{
    public partial class ProductPictureMap : NopEntityTypeConfiguration<ProductPicture>
    {
        public override void Configure(EntityTypeBuilder<ProductPicture> builder)
        {
            base.Configure(builder);
            builder.ToTable("Product_Picture_Mapping");
            builder.HasKey(pp => pp.Id);

            builder.HasOne(pp => pp.Picture)
                .WithMany()
                .HasForeignKey(pp => pp.PictureId)
                .IsRequired(true);


            builder.HasOne(pp => pp.Product)
                .WithMany(p => p.ProductPictures)
                .HasForeignKey(pp => pp.ProductId)
                .IsRequired(true);
        }
    }
}