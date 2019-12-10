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

            builder.Property(note => note.Note).IsNullable(false);
            builder.Property(note => note.VendorId);
            builder.Property(note => note.CreatedOnUtc);
        }

        #endregion
    }
}