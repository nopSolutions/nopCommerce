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
            this.Property(p => p.ShortDescription).IsMaxLength();
            this.Property(p => p.FullDescription).IsMaxLength();
            this.Property(p => p.AdminComment).IsMaxLength();
            this.Property(p => p.MetaKeywords).HasMaxLength(400);
            this.Property(p => p.MetaDescription);
            this.Property(p => p.MetaTitle).HasMaxLength(400);
            this.Property(p => p.SeName).HasMaxLength(200);
        }
    }
}