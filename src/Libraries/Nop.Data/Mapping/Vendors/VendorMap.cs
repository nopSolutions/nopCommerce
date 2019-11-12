using LinqToDB.Mapping;
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
        public override void Configure(EntityMappingBuilder<Vendor> builder)
        {
            builder.HasTableName(nameof(Vendor));

            builder.HasColumn(vendor => vendor.Name).IsColumnRequired();
            builder.Property(vendor => vendor.Name).HasLength(400);
            builder.Property(vendor => vendor.Email).HasLength(400);
            builder.Property(vendor => vendor.MetaKeywords).HasLength(400);
            builder.Property(vendor => vendor.MetaTitle).HasLength(400);
            builder.Property(vendor => vendor.PageSizeOptions).HasLength(200);

            builder.Property(vendor => vendor.Description);
            builder.Property(vendor => vendor.PictureId);
            builder.Property(vendor => vendor.AddressId);
            builder.Property(vendor => vendor.AdminComment);
            builder.Property(vendor => vendor.Active);
            builder.Property(vendor => vendor.Deleted);
            builder.Property(vendor => vendor.DisplayOrder);
            builder.Property(vendor => vendor.MetaDescription);
            builder.Property(vendor => vendor.PageSize);
            builder.Property(vendor => vendor.AllowCustomersToSelectPageSize);
        }

        #endregion
    }
}