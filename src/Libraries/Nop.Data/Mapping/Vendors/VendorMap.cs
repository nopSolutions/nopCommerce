using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Vendors;

namespace Nop.Data.Mapping.Vendors
{
    /// <summary>
    /// Represents a vendor mapping configuration
    /// </summary>
    public partial class VendorMap : NopEntityTypeConfiguration<Vendor>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityTypeBuilder<Vendor> builder)
        {
            builder.ToTable(nameof(Vendor));
            builder.HasKey(vendor => vendor.Id);

            builder.Property(vendor => vendor.Name).HasMaxLength(400).IsRequired();
            builder.Property(vendor => vendor.Email).HasMaxLength(400);
            builder.Property(vendor => vendor.MetaKeywords).HasMaxLength(400);
            builder.Property(vendor => vendor.MetaTitle).HasMaxLength(400);
            builder.Property(vendor => vendor.PageSizeOptions).HasMaxLength(200);

            base.Configure(builder);
        }

        #endregion
    }
}