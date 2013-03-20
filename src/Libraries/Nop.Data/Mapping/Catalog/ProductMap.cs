using System.Data.Entity.ModelConfiguration;
using Nop.Core.Domain.Catalog;

namespace Nop.Data.Mapping.Catalog
{
    public partial class ProductMap : EntityTypeConfiguration<Product>
    {
        public ProductMap()
        {
            this.ToTable("Product");
            this.HasKey(p => p.Id);
            this.Property(p => p.Name).IsRequired().HasMaxLength(400);
            this.Property(p => p.MetaKeywords).HasMaxLength(400);
            this.Property(p => p.MetaTitle).HasMaxLength(400);

            this.HasMany(p => p.ProductTags)
                .WithMany()
                .Map(m => m.ToTable("Product_ProductTag_Mapping"));

            this.HasOptional(p => p.Vendor)
                .WithMany()
                .HasForeignKey(p => p.VendorId).WillCascadeOnDelete(false);
        }
    }
}