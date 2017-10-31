using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Vendors;

namespace Nop.Data.Mapping.Vendors
{
    /// <summary>
    /// Mapping class
    /// </summary>
    public partial class VendorNoteMap : NopEntityTypeConfiguration<VendorNote>
    {
        public override void Configure(EntityTypeBuilder<VendorNote> builder)
        {
            base.Configure(builder);
            builder.ToTable("VendorNote");
            builder.HasKey(vn => vn.Id);
            builder.Property(vn => vn.Note).IsRequired();

            builder.HasOne(vn => vn.Vendor)
                .WithMany(v => v.VendorNotes)
                .IsRequired(true)
                .HasForeignKey(vn => vn.VendorId);
        }
    }
}