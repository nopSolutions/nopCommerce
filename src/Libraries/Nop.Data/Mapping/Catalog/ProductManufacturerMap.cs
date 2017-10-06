using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Catalog;

namespace Nop.Data.Mapping.Catalog
{
    public partial class ProductManufacturerMap : NopEntityTypeConfiguration<ProductManufacturer>
    {
        public override void Configure(EntityTypeBuilder<ProductManufacturer> builder)
        {
            base.Configure(builder);
            builder.ToTable("Product_Manufacturer_Mapping");
            builder.HasKey(pm => pm.Id);

            builder.HasOne(pm => pm.Manufacturer)
                .WithMany()
                .HasForeignKey(pm => pm.ManufacturerId);


            builder.HasOne(pm => pm.Product)
                .WithMany(p => p.ProductManufacturers)
                .HasForeignKey(pm => pm.ProductId);
        }
    }
}