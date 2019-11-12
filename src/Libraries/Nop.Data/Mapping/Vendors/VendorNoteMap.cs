using LinqToDB.Mapping;
using Nop.Core.Domain.Vendors;

namespace Nop.Data.Mapping.Vendors
{
    /// <summary>
    /// Represents a vendor note mapping configuration
    /// </summary>
    public partial class VendorNoteMap : NopEntityTypeConfiguration<VendorNote>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityMappingBuilder<VendorNote> builder)
        {
            builder.HasTableName(nameof(VendorNote));

            builder.HasColumn(note => note.Note).IsColumnRequired();
            builder.Property(note => note.VendorId);
            builder.Property(note => note.CreatedOnUtc);

            //TODO: 239 Try to add ForeignKey
            //builder.HasOne(note => note.Vendor)
            //    .WithMany(vendor => vendor.VendorNotes)
            //    .HasForeignKey(note => note.VendorId)
            //    .IsColumnRequired();
        }

        #endregion
    }
}