using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Vendors;

namespace Nop.Data.Mapping.Vendors
{
    public partial class VendorMap : NopEntityTypeConfiguration<Vendor>
    {
        public override void Configure(EntityTypeBuilder<Vendor> builder)
        {
            base.Configure(builder);
            builder.ToTable("Vendor");
            builder.HasKey(v => v.Id);

            builder.Property(v => v.Name).IsRequired().HasMaxLength(400);
            builder.Property(v => v.Email).HasMaxLength(400);
            builder.Property(v => v.MetaKeywords).HasMaxLength(400);
            builder.Property(v => v.MetaTitle).HasMaxLength(400);
            builder.Property(v => v.PageSizeOptions).HasMaxLength(200);
        }
    }
}