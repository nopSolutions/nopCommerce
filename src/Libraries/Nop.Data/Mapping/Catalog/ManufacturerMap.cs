using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Catalog;

namespace Nop.Data.Mapping.Catalog
{
    public partial class ManufacturerMap : NopEntityTypeConfiguration<Manufacturer>
    {
        public override void Configure(EntityTypeBuilder<Manufacturer> builder)
        {
            base.Configure(builder);
            builder.ToTable("Manufacturer");
            builder.HasKey(m => m.Id);
            builder.Property(m => m.Name).IsRequired().HasMaxLength(400);
            builder.Property(m => m.MetaKeywords).HasMaxLength(400);
            builder.Property(m => m.MetaTitle).HasMaxLength(400);
            builder.Property(m => m.PriceRanges).HasMaxLength(400);
            builder.Property(m => m.PageSizeOptions).HasMaxLength(200);
        }
    }
}