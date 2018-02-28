using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Vendors;

namespace Nop.Data.Mapping.Vendors
{
    /// <summary>
    /// Mapping class
    /// </summary>
    public partial class VendorAttributeValueMap : NopEntityTypeConfiguration<VendorAttributeValue>
    {
        public override void Configure(EntityTypeBuilder<VendorAttributeValue> builder)
        {
            base.Configure(builder);
            builder.ToTable("VendorAttributeValue");
            builder.HasKey(vendorAttributeValue => vendorAttributeValue.Id);
            builder.Property(vendorAttributeValue => vendorAttributeValue.Name).IsRequired().HasMaxLength(400);
            builder.HasOne(vendorAttributeValue => vendorAttributeValue.VendorAttribute)
                .WithMany(vendorAttribute => vendorAttribute.VendorAttributeValues)
                .HasForeignKey(vendorAttributeValue => vendorAttributeValue.VendorAttributeId)
                .IsRequired();
        }
    }
}