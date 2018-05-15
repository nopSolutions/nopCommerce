using Nop.Core.Domain.Vendors;

namespace Nop.Data.Mapping.Vendors
{
    /// <summary>
    /// Mapping class
    /// </summary>
    public partial class VendorNoteMap : NopEntityTypeConfiguration<VendorNote>
    {
        /// <summary>
        /// Ctor
        /// </summary>
        public VendorNoteMap()
        {
            this.ToTable("VendorNote");
            this.HasKey(vn => vn.Id);
            this.Property(vn => vn.Note).IsRequired();

            this.HasRequired(vn => vn.Vendor)
                .WithMany(v => v.VendorNotes)
                .HasForeignKey(vn => vn.VendorId);
        }
    }
}